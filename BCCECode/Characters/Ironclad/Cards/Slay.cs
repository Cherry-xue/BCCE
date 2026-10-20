using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Ironclad.Cards;

// 杀戮-获得1点力量，消耗1张牌。造成5点伤害2次，每次额外造成你最大生命与你当前生命差值的伤害。
public class Slay : IroncladCard
{
    public Slay() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    //定义卡牌基本属性：2能量，攻击，稀有，目标为任意敌人,图鉴可见
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1m),
        new CalculationBaseVar(5m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => card.Owner.Creature.MaxHp - card.Owner.Creature.CurrentHp)
    ];
    //定义可变参数:力量数值，初始为1;伤害基准数值，初始为5;每点生命差值提供的额外伤害，初始为1;计算后的伤害数值
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];
    //定义提示:提示力量和消耗的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars.Strength.BaseValue, Owner.Creature, this);
        foreach (CardModel item in await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1), context: choiceContext, player: Owner, filter: null, source: this))
        {
            await CardCmd.Exhaust(choiceContext, item);
        }
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this, cardPlay).Targeting(cardPlay.Target).WithHitCount(2)
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
    }
    //卡牌效果:获得力量，消耗手牌中的一张牌，然后对目标造成2次伤害，每次伤害为伤害基准数值加上最大生命与当前生命的差值
    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2m);
    }
    //升级效果:伤害基准数值增加2
}
