using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Silent.Cards;

// 埋伏-获得8点格挡，在本回合给你手牌中的一张攻击牌添加奇巧。
public class Ambush : SilentCard
{
    public Ambush() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    //定义卡牌基本属性：1能量，技能，常见，目标为自身,图鉴可见
    {
    }
    public override bool GainsBlock => true;
    //定义卡牌是否获得格挡：是
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8, ValueProp.Move)
    ];
    //定义可变参数:格挡数值，初始为8
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Sly)
    ];
    //定义提示:提示奇巧的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        CardModel cardModel = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1), context: choiceContext, player: Owner, filter: (CardModel card) => card.Type == CardType.Attack && !card.IsSlyThisTurn, source: this)).FirstOrDefault();
        if (cardModel != null)
        {
            CardCmd.ApplySingleTurnSly(cardModel);
        }
    }
    //卡牌效果:
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
    //升级效果:格挡数值增加3
}
