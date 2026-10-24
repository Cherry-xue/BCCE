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

// 无声-在你的回合结束时，每有一张手牌，就获得1点敏捷,下个回合结束时,失去等量敏捷
public sealed class AphoniaPower : SilentPower
{
    private int cardamount = 0;
    //定义一个私有字段cardamount，用于显示本次结算授予的临时敏捷数量
    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益
    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    //定义实例类型：独立实例
    public override int DisplayAmount => cardamount;
    //定义显示数值：最近一次结算授予的临时敏捷数量
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<DexterityPower>()
    ];
    //定义额外的悬停提示：显示敏捷的悬停提示
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            IReadOnlyList<CardModel> cards = PileType.Hand.GetPile(Owner.Player).Cards;
            cardamount = cards.Count * Amount;
            if (cardamount > 0)
            {
                Flash();
                await PowerCmd.Apply<AphoniaDexterityPower>(choiceContext, Owner, cardamount, Owner, null);
            }
            InvokeDisplayAmountChanged();
        }
    }
    //在自己的回合结束前触发：按当前手牌数授予独立的临时敏捷；旧敏捷由临时能力在更早的结束阶段撤回
}
