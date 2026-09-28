using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode.Characters.Silent.Cards;

// 虚影-丢弃所有手牌，每丢弃3张牌获得1点敏捷。
public class Phantom : SilentCard
{
    public Phantom() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    //定义卡牌基本属性：0能量，技能，罕见稀有度，目标为自身,图鉴可见
    {
    }
    public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    //卡牌关键词:消耗
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];
    //定义可变参数:卡牌数量，初始值为3
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<DexterityPower>()
    ];
    //定义提示:提示敏捷的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<CardModel> enumerable = PileType.Hand.GetPile(Owner).Cards.ToList();
        int handSize = enumerable.Count();
        await CardCmd.Discard(choiceContext, enumerable);
        await Cmd.CustomScaledWait(0f, 0.25f);
        int dexterityAmount = handSize / DynamicVars.Cards.IntValue;
        if (dexterityAmount > 0)
        {
            await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature, dexterityAmount, Owner.Creature, this);
        }
    }
    //卡牌效果:
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
    //升级效果:添加保留关键词
}
