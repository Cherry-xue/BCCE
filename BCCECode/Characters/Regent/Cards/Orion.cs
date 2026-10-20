using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Regent.Cards;

// 猎户座-造成14点伤害。打出时每有一张手牌，铸造3。
public class Orion : RegentCard
{
    public Orion() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    //定义卡牌基本属性：2能量，攻击，罕见，目标为任意敌人,图鉴可见
    {
    }
    public override int CanonicalStarCost => 1;
    //定义卡牌的辉星消耗：1
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14m, ValueProp.Move),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(3m),
        new CalculatedVar("CalculatedForge").WithMultiplier((card, _) => PileType.Hand.GetPile(card.Owner).Cards.Count(c => c != card))
    ];
    //定义可变参数:伤害数值，初始为14;铸造基准数值，初始为0;每张手牌提供的铸造数值，初始为3;计算后的铸造数值
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => HoverTipFactory.FromForge();
    //定义提示:提示铸造的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
            .Execute(choiceContext);
        decimal amount = ((CalculatedVar)DynamicVars["CalculatedForge"]).Calculate(cardPlay.Target);
        await ForgeCmd.Forge(amount, Owner, this);
    }
    //卡牌效果:对目标造成伤害，然后按手牌数量铸造
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        EnergyCost.UpgradeBy(-1);
    }
    //升级效果:伤害数值增加3,能量消耗减少1
}
