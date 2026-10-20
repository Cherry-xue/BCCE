using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode.Characters.Ironclad.Powers;

// 弱肉强食-每当你获得力量时，使随机一名敌人失去等量力量
public sealed class PredationPower : IroncladPower
{
    private int turnCounter = 1;
    //定义一个私有字段turnCounter，用于记录回合数，回合开始时加一，以此交替施加两个同名的弱者能力
    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益
    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    //定义实例类型：独立实例
    public override int DisplayAmount => Amount;
    //定义显示数值:减力量效果的持续回合数
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    //定义额外的悬停提示：显示力量的悬停提示
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner))
        {
            turnCounter++;
        }
    }
    //在回合开始后触发：回合计数加一，使相邻两个回合施加的弱者能力落在两个不同的同名能力上
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature applier, CardModel cardSource)
    {
        if (power is not StrengthPower || power.Owner != Owner || amount <= 0m)
        {
            return;
        }
        IReadOnlyList<Creature> enemies = CombatState.HittableEnemies;
        if (enemies.Count == 0)
        {
            return;
        }
        Creature enemy = Owner.Player.RunState.Rng.CombatTargets.NextItem(enemies);
        await PowerCmd.Apply<StrengthPower>(choiceContext, enemy, -amount, Owner, null);
        if (Amount == 1)
        {
            WeaklingPower weakling = await PowerCmd.Apply<WeaklingPower>(choiceContext, enemy, amount, Owner, null);
            if (weakling != null)
            {
                weakling.SetDuration(Amount);
            }
        }
        else
        {
            if (turnCounter % 2 == 1)
            {
                WeaklingPower weakling = await PowerCmd.Apply<WeaklingPower1>(choiceContext, enemy, amount, Owner, null);
                if (weakling != null)
                {
                    weakling.SetDuration(Amount);
                }
            }
            else
            {
                WeaklingPower2 weakling = await PowerCmd.Apply<WeaklingPower2>(choiceContext, enemy, amount, Owner, null);
                if (weakling != null)
                {
                    weakling.SetDuration(Amount);
                }
            }
        }
    }
    //在能力层数变化后触发：若自己获得力量，则使随机一名敌人失去等量力量，并按回合奇偶交替施加弱者能力
}
