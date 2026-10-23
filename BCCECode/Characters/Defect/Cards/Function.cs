using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace BCCE.BCCECode.Characters.Defect.Cards;

// 函数-生成1个玻璃充能球，选择抽牌堆中的一张卡丢弃。
public class Function : DefectCard
{
    public Function() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    //定义卡牌基本属性：1能量，技能，普通，目标为自身,图鉴可见
    {
    }
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromOrb<GlassOrb>()
    ];
    //定义提示:提示玻璃充能球的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await OrbCmd.Channel<GlassOrb>(choiceContext, Owner);
        CardModel selectedCard = (await CardSelectCmd.FromCombatPile(choiceContext, PileType.Draw.GetPile(Owner), Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1))).FirstOrDefault();
        if (selectedCard != null)
        {
            await CardCmd.Discard(choiceContext, selectedCard);
        }
    }
    //卡牌效果:生成一个玻璃充能球，再从抽牌堆中选择一张牌丢弃
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    //升级效果:能量消耗减少1
}
