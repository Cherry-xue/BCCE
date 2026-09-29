using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Silent.Cards;

// 穿透-造成2点伤害2次，下回合抽1张牌。
public class Penetrate : SilentCard
{
    public Penetrate() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    //定义卡牌基本属性：1能量，攻击，常见，目标为任意敌人,图鉴可见
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2, ValueProp.Move),
        new CardsVar(1)
    ];
    //定义可变参数:伤害数值,初始为1;卡牌数量，初始值为1;能量数值，初始值为1
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        EnergyHoverTip
    ];
    //定义提示:提示能量的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target).WithHitCount(2)
            .WithHitFx("vfx/vfx_attack_slash", null, "blunt_attack.mp3")
            .Execute(choiceContext);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, Owner.Creature, DynamicVars.Cards.BaseValue, Owner.Creature, null);
    }
    //卡牌效果:
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
    //升级效果:伤害数值增加1
}
