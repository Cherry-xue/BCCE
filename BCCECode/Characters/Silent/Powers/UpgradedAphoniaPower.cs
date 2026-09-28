using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode.Characters.Silent.Powers;

// 无声-在你的回合结束时，每有一张手牌，就获得2点敏捷,下个回合结束时,失去等量敏捷
public sealed class UpgradedAphoniaPower : SilentPower
{
    private int cardamount = 0;
    //定义一个私有字段cardamount，用于存储玩家的手牌数量
    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益
    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    //定义实例类型：单例
    public override int DisplayAmount => cardamount;
    //定义显示数值:玩家的手牌数量
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<DexterityPower>()
    ];
    //定义额外的悬停提示：显示敏捷的悬停提示
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, -cardamount, Owner, null);
            IReadOnlyList<CardModel> cards = PileType.Hand.GetPile(Owner.Player).Cards;
            if (cards.Count != 0)
            {
                cardamount = cards.Count * 2;
                Flash();
                await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, cardamount, Owner, null);
            }
            else
            {
                cardamount = 0;
            }
            InvokeDisplayAmountChanged();
        }
    }
    //在回合结束前触发的事件中，检查当前回合是否为玩家的回合，如果是，则根据手牌数量调整敏捷值，并更新显示数值
}
