using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Random;

namespace BCCE.BCCECode.Characters.Regent.Cards;

// 星云-抽2张牌，选择你手牌中的一张牌变化为随机0耗能无色牌。
public class Nebula : RegentCard
{
    public Nebula() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    //定义卡牌基本属性：1能量，技能，普通，目标为自身,图鉴可见
    {
    }
    public override int CanonicalStarCost => 1;
    //定义卡牌的辉星消耗：1
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    //定义卡牌关键词：消耗
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];
    //定义可变参数:抽牌数量，初始为2
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        CardModel selectedCard = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1), null, this)).FirstOrDefault();
        if (selectedCard == null)
        {
            return;
        }
        List<CardModel> candidates = ModelDb.CardPool<ColorlessCardPool>()
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where((CardModel c) => c.EnergyCost.Canonical == 0)
            .ToList();
        if (candidates.Count == 0)
        {
            return;
        }
        //使用局内确定性随机源，保证多人联机各端变形结果一致
        Rng rng = Owner.RunState.Rng.CombatCardGeneration;
        CardModel replacement = CombatState.CreateCard(candidates[rng.NextInt(candidates.Count)], Owner);
        await CardCmd.Transform(selectedCard, replacement);
    }
    //卡牌效果:抽牌后选择一张手牌，将其变形为一张随机的0耗能无色牌
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    //升级效果:能量消耗减少1
}
