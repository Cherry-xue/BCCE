using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Ironclad.Powers;

// 聚力打击-在你的回合结束时，本回合每使用1点能量，获得2点力量，下个回合结束时，失去等量力量
public sealed class ChargedStrikePower : IroncladPower
{
    private int strengthamount = 0;
    //定义一个私有字段strengthamount，用于存储本回合结束时给予的力量数值
    private bool isGranted = false;
    //定义一个私有字段isGranted，用于标记是否已经在回合结束时给予过力量
    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益
    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    //定义实例类型：独立实例
    public override int DisplayAmount => strengthamount;
    //定义显示数值:本回合结束时给予的力量数值
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("StrengthAmounts", 0m)
    ];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    //定义额外的悬停提示：显示力量的悬停提示
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int energySpent = CombatManager.Instance.History.Entries.OfType<EnergySpentEntry>().Where(e => e.Actor == Owner && e.HappenedThisTurn(CombatState)).Sum(e => e.Amount);
        strengthamount = energySpent * Amount;
        DynamicVars["StrengthAmounts"].BaseValue = strengthamount;
        InvokeDisplayAmountChanged();
    }//在卡牌打出后触发：统计本回合使用的能量并更新显示数值
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            if (!isGranted)
            {
                int energySpent = CombatManager.Instance.History.Entries.OfType<EnergySpentEntry>().Where(e => e.Actor == Owner && e.HappenedThisTurn(CombatState)).Sum(e => e.Amount);
                strengthamount = energySpent * Amount;
                if (strengthamount != 0)
                {
                    Flash();
                    await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, strengthamount, Owner, null);
                }
                isGranted = true;
                DynamicVars["StrengthAmounts"].BaseValue = strengthamount;
                InvokeDisplayAmountChanged();
            }
            else
            {
                await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -strengthamount, Owner, null);
                await PowerCmd.Remove<ChargedStrikePower>(Owner);
            }
        }
    }
    //在回合结束前触发：第一次结算时统计本回合使用的能量并给予力量，下个回合结束时失去等量力量并移除本能力
}
