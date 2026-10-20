using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode.Characters.Regent.Powers;

// 潮汐-使目标在本回合失去力量，回合结束时返还
public sealed class TidePower : RegentPower
{
    public override PowerType Type => PowerType.Debuff;
    //定义能力类型：减益
    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器
    public override int DisplayAmount => Amount;
    //定义显示数值:失去的力量层数
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    //定义额外的悬停提示：显示力量的悬停提示
    public override async Task BeforeApplied(Creature target, decimal amount, Creature applier, CardModel cardSource)
    {
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), target, -amount, applier, cardSource);
    }
    //在施加时触发：立刻使目标失去等量力量
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, Amount, Owner, null);
            await PowerCmd.Remove(this);
        }
    }
    //在回合结束前触发：返还本回合失去的力量并移除本能力
}
