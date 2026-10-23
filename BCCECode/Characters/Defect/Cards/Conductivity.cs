using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Defect.Cards;

// 导电-造成8点伤害。每有1名敌人，触发1次你最右侧的闪电充能球的被动
public class Conductivity : DefectCard
{
    public Conductivity() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    //定义卡牌基本属性：1能量，攻击，普通，目标为任意敌人,图鉴可见
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move)
    ];
    //定义可变参数:伤害数值，初始为8
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromOrb<LightningOrb>()
    ];
    //定义提示:提示闪电充能球的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash", null, "slash_attack.mp3")
            .Execute(choiceContext);
        LightningOrb lightningOrb = Owner.PlayerCombatState.OrbQueue.Orbs.OfType<LightningOrb>().FirstOrDefault();
        if (lightningOrb == null)
        {
            return;
        }
        for (int i = 0; i < CombatState.HittableEnemies.Count; i++)
        {
            await OrbCmd.Passive(choiceContext, lightningOrb, null);
        }
    }
    //卡牌效果:造成伤害，然后每有一名敌人就触发一次最右侧闪电充能球的被动；目标传空值，由闪电球自己随机选择目标
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
    //升级效果:伤害数值增加3
}
