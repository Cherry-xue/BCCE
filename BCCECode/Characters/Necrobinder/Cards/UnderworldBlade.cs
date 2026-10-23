using System;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Necrobinder.Cards;

// 冥界刀锋-对所有敌人造成4点伤害2次，分别给予每个敌人最大生命值15%的灾厄，这张牌的耗能增加1。
public class UnderworldBlade : NecrobinderCard
{
    public UnderworldBlade() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, true)
    //定义卡牌基本属性：2能量，攻击，罕见，目标为所有敌人,图鉴可见
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new DynamicVar("DoomPercent", 15m)
    ];
    //定义可变参数:伤害数值，初始为4;给予的灾厄占敌人最大生命的百分比，初始为15
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<DoomPower>()
    ];
    //定义提示:提示灾厄的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).TargetingAllOpponents(CombatState)
            .WithHitCount(2)
            .WithHitFx("vfx/vfx_attack_slash", null, "slash_attack.mp3")
            .Execute(choiceContext);
        foreach (Creature enemy in CombatState.HittableEnemies)
        {
            decimal doom = Math.Floor(enemy.MaxHp * DynamicVars["DoomPercent"].BaseValue / 100m);
            await PowerCmd.Apply<DoomPower>(choiceContext, enemy, doom, Owner.Creature, this);
        }
        EnergyCost.AddThisCombat(1);
    }
    //卡牌效果:对所有敌人造成两次伤害，再按每个敌人各自的最大生命给予灾厄，最后本场战斗中这张牌的耗能增加1
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    //升级效果:减少1点能量消耗
}
