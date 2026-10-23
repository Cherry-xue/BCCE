#nullable enable
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace BCCE.BCCECode.Characters.Necrobinder.Powers;

// 出窍-攻击牌获得虚无。
public sealed class OutOfBodyPower : NecrobinderPower
{
    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益
    public override PowerStackType StackType => PowerStackType.Single;
    //定义叠加类型:单一
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];
    //定义提示:提示虚无的相关信息
    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (!(power is OutOfBodyPower))
        {
            return Task.CompletedTask;
        }
        if (power.Owner != Owner)
        {
            return Task.CompletedTask;
        }
        IEnumerable<CardModel> enumerable = Owner.Player?.PlayerCombatState?.AllCards ?? Array.Empty<CardModel>();
        foreach (CardModel item in enumerable)
        {
            TryAddEthereal(item);
        }
        return Task.CompletedTask;
    }
    //在能力数值变化后触发：为所有牌堆中的攻击牌添加虚无
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        TryAddEthereal(card);
        return Task.CompletedTask;
    }
    //在牌进入战斗后触发：为该牌添加虚无
    private void TryAddEthereal(CardModel card)
    {
        if (card.Owner == Owner.Player && card.Type == CardType.Attack)
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Ethereal);
        }
    }
    //定义私有方法：为指定牌添加虚无
}
