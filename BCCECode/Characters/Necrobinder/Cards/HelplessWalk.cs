using BCCE.BCCECode.Characters.Necrobinder.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;

namespace BCCE.BCCECode.Characters.Necrobinder.Cards;

// 奈何行-本回合内，你每打出3张灵魂，获得1点能量。
public class HelplessWalk : NecrobinderCard
{
    public HelplessWalk() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    //定义卡牌基本属性：2能量，技能，罕见，目标为自身,图鉴可见
    {
    }
    public override List<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    //卡牌关键词：消耗
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromCard<Soul>()
    ];
    //定义提示:提示灵魂卡牌的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<HelplessWalkPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }
    //卡牌效果:获得奈何行能力，其会在本回合内统计打出的灵魂数量并结算能量
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    //升级效果:减少1点能量消耗
}
