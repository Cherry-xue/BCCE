using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Defect.Cards;

// 反执行-造成5点伤害2次，触发最右侧充能球的被动，激发最左侧的充能球。
public class CounterExecute : DefectCard
{
    public CounterExecute() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    //定义卡牌基本属性：1能量，攻击，罕见，目标为任意敌人,图鉴可见
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new RepeatVar(2)
    ];
    //定义可变参数:伤害数值，初始为5;攻击次数，初始为2
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .WithHitFx("vfx/vfx_attack_slash", null, "slash_attack.mp3")
            .Execute(choiceContext);
        OrbModel rightmostOrb = Owner.PlayerCombatState.OrbQueue.Orbs.FirstOrDefault();
        if (rightmostOrb != null)
        {
            await OrbCmd.Passive(choiceContext, rightmostOrb, null);
        }
        await OrbCmd.EvokeLast(choiceContext, Owner);
    }
    //卡牌效果:造成两次伤害，触发最右侧充能球的被动，激发最左侧的充能球
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
    //升级效果:伤害数值增加2
}
