using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Regent.Cards;

// 萤火-造成6点伤害，获得1辉星。如果上一张打出的牌消耗或获得了辉星，则额外造成4点伤害并抽1张牌。
public class Firefly : RegentCard
{
    public Firefly() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    //定义卡牌基本属性：1能量，攻击，普通，目标为任意敌人,图鉴可见
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new StarsVar(1),
        new DynamicVar("BonusDamage", 4m),
        new CardsVar(1)
    ];
    //定义可变参数:伤害数值，初始为6;获得的辉星数量，初始为1;额外伤害数值，初始为4;抽牌数量，初始为1
    private int starsBeforeCard;
    //定义一个私有字段starsBeforeCard，用于记录卡牌打出前的辉星数
    private bool lastCardUsedStars;
    //定义一个私有字段lastCardUsedStars，用于记录上一张打出的牌是否消耗或获得了辉星
    protected override bool ShouldGlowGoldInternal => lastCardUsedStars;
    //定义卡牌发光条件：当上一张打出的牌消耗或获得了辉星时，卡牌发光
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner)
        {
            starsBeforeCard = Owner.PlayerCombatState.Stars;
        }
        return Task.CompletedTask;
    }
    //在卡牌打出前触发：记录辉星数作为基准。能力比卡牌先响应此钩子，因此封印王座给予的辉星不会被计入
    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner && cardPlay.IsLastInSeries)
        {
            lastCardUsedStars = cardPlay.Resources.StarsSpent > 0 || Owner.PlayerCombatState.Stars > starsBeforeCard;
        }
        return Task.CompletedTask;
    }
    //在卡牌打出后触发：实际消耗了辉星，或辉星数比打出前变多，则记为这张牌消耗或获得了辉星
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash", null, "slash_attack.mp3")
            .Execute(choiceContext);
        await PlayerCmd.GainStars(DynamicVars.Stars.BaseValue, Owner);
        if (lastCardUsedStars)
        {
            await DamageCmd.Attack(DynamicVars["BonusDamage"].BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash", null, "slash_attack.mp3")
                .Execute(choiceContext);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }
    }
    //卡牌效果:造成伤害并获得辉星，若上一张打出的牌消耗或获得了辉星，则额外造成一段伤害并抽牌
    protected override void OnUpgrade()
    {
        DynamicVars.Stars.UpgradeValueBy(1m);
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
    //升级效果:获得的辉星数量增加1,伤害数值增加3
}
