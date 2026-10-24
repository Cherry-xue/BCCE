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

namespace BCCE.BCCECode.Characters.Ironclad.Powers;

// 聚力打击-在你的回合结束时，本回合每使用1点能量，获得2点力量，下个回合结束时，失去等量力量
public sealed class ChargedStrikePower : IroncladPower
{
    private int strengthamount = 0;
    //定义一个私有字段strengthamount，用于存储本回合结束时给予的力量数值
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
    //定义可变参数：本回合结束时预计获得的力量数值
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    //定义额外的悬停提示：显示力量的悬停提示
    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int energySpent = CombatManager.Instance.History.Entries.OfType<EnergySpentEntry>().Where(e => e.Actor == Owner && e.HappenedThisTurn(CombatState)).Sum(e => e.Amount);
        strengthamount = energySpent * Amount;
        DynamicVars["StrengthAmounts"].BaseValue = strengthamount;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
    //在卡牌打出后触发：统计本回合使用的能量并更新预计获得的力量，不负责记录已经授予的临时力量
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            int energySpent = CombatManager.Instance.History.Entries.OfType<EnergySpentEntry>().Where(e => e.Actor == Owner && e.HappenedThisTurn(CombatState)).Sum(e => e.Amount);
            strengthamount = energySpent * Amount;
            if (strengthamount > 0)
            {
                Flash();
                await PowerCmd.Apply<ChargedStrikeStrengthPower>(choiceContext, Owner, strengthamount, Owner, null);
            }
            await PowerCmd.Remove(this);
        }
    }
    //在回合结束前触发：按本回合消耗的能量授予独立的临时力量，然后仅移除本次聚力打击的待结算实例
}
