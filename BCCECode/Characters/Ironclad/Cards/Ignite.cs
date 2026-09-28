using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode.Characters.Ironclad.Cards;

// 点燃-消耗抽牌堆中的一张牌,获得力量
public class Ignite : IroncladCard
{
    public Ignite() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    //定义卡牌基本属性：1能量，能力，常见稀有度，目标为自己,图鉴可见
    {
    }
    public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    //卡牌关键词：消耗
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new CardsVar(1),
        new PowerVar<StrengthPower>(1m)
    ];
    //定义可变参数:卡牌数量，初始值为1;获得的力量数值，初始值为1
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    //定义提示:提示力量的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> selection = (await CardSelectCmd.FromCombatPile(choiceContext, PileType.Draw.GetPile(Owner), Owner, new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, DynamicVars.Cards.IntValue))).ToList();
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        foreach (CardModel item in selection)
        {
            await CardCmd.Exhaust(choiceContext, item);
        }
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars.Strength.BaseValue, Owner.Creature, this);
    }
    //卡牌效果:
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    //升级效果:减少1点能量消耗
}
