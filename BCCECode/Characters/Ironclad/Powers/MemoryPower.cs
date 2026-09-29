using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode.Characters.Ironclad.Powers;

// 回忆-在你的回合结束时，消耗所有非消耗堆的状态牌，每消耗两张，获得1点力量
public sealed class MemoryPower : IroncladPower
{
    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益
    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    //定义额外的悬停提示：显示力量的悬停提示
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            List<CardModel> list = GetStatuses(Owner.Player).ToList();
            int powerCount = list.Count / 2;
            foreach (CardModel item in list)
            {
                await CardCmd.Exhaust(choiceContext, item);
            }
            if (powerCount > 0)
            {
                await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, powerCount, Owner, null);
            }
            await PowerCmd.Remove<MemoryPower>(Owner);
        }
    }
    //在回合结束前触发的事件中，检查当前回合是否为玩家的回合，如果是，则根据手牌数量调整敏捷值，并更新显示数值
    private static IEnumerable<CardModel> GetStatuses(Player owner)
    {
        return owner.PlayerCombatState.AllCards.Where((CardModel c) => c.Type == CardType.Status && c.Pile.Type != PileType.Exhaust);
    }
    //获取玩家所有非消耗堆的状态牌
}
