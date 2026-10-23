using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using BCCE.BCCECode.Characters.Necrobinder.Powers;

namespace BCCE.BCCECode.Characters.Necrobinder.Cards;

// 出窍-使所有牌堆中的攻击牌在打出之前的耗能减少1,攻击牌获得虚无
public class OutOfBody : NecrobinderCard
{
    public OutOfBody() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    //定义卡牌基本属性：3能量，能力，稀有，目标为自身,图鉴可见
    {
    }
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Eternal
    ];
    //定义卡牌关键词：永恒
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];
    //定义提示:提示虚无的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        foreach (CardModel item in Owner.PlayerCombatState.AllCards)
        {
            if (item.Type != CardType.Attack)
            {
                continue;
            }
            item.EnergyCost.AddUntilPlayed(-1);
        }
        await PowerCmd.Apply<OutOfBodyPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }
    //卡牌效果:使所有牌堆中的攻击牌在打出之前的耗能减少1,并获得出窍能力
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
    //升级效果:添加保留关键词
}
