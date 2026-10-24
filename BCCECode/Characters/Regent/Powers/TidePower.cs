using BCCE.BCCECode.Characters.Regent.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode.Characters.Regent.Powers;

// 潮汐-使目标在本回合失去力量，回合结束时返还
public sealed class TidePower : RegentPower, ITemporaryPower
{
    public override PowerType Type => PowerType.Debuff;
    //定义能力类型：减益
    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器
    public AbstractModel OriginModel => ModelDb.Card<Tide>();
    //定义临时能力来源：潮汐
    public PowerModel InternallyAppliedPower => ModelDb.Power<StrengthPower>();
    //声明本临时能力关联原版力量，供原版识别临时减力量与内部力量变化的关系
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    //定义额外的悬停提示：显示力量的悬停提示
    public override async Task BeforeApplied(Creature target, decimal amount, Creature applier, CardModel cardSource)
    {
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), target, -amount, applier, cardSource, silent: true);
    }
    //在施加时触发：立刻使目标失去等量力量
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature applier, CardModel cardSource)
    {
        if (power == this && amount != Amount)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -amount, applier, cardSource, silent: true);
        }
    }
    //在层数改变后触发：首次施加已由BeforeApplied处理，后续叠加或减少时同步调整目标的力量
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, Amount, Owner, null);
        }
    }
    //在回合结束前触发：返还本回合失去的力量并移除本能力
}
