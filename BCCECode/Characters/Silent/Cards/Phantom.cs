using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Silent.Cards;

// 虚影-丢弃所有手牌，每丢弃3张牌获得1点敏捷，每丢弃一张牌获得一次1点格挡。
public class Phantom : SilentCard
{
    public Phantom() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    //定义卡牌基本属性：0能量，技能，罕见，目标为自身,图鉴可见
    {
    }
    public override bool GainsBlock => true;
    //定义卡牌是否获得格挡：是
    public override List<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust,
        CardKeyword.Retain
    ];
    //卡牌关键词:消耗,保留
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new BlockVar(1, ValueProp.Move)
    ];
    //定义可变参数:卡牌数量，初始值为3,格挡数值，初始为1
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
        for (int i = 0; i < handSize; i++)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        }
    }
    //卡牌效果:
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(-1m);
    }
    //升级效果:卡牌数量减少1
}
