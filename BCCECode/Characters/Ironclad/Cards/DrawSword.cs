using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Ironclad.Cards;

// 拔剑-对所有敌人造成4点伤害，获得等于敌人意图攻击的最小伤害值一半的格挡。
public class DrawSword : IroncladCard
{
    public DrawSword() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies, true)
    //定义卡牌基本属性：1能量，攻击，常见，目标为所有敌人,图鉴可见
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4, ValueProp.Move),
        new BlockVar(0, ValueProp.Unpowered)
    ];
    //定义可变参数:伤害数值,初始为4;格挡数值，初始值为0
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).TargetingAllOpponents(CombatState).WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3").Execute(choiceContext);
        int? minimumIntentDamage = null;
        if (cardPlay.Player.Creature.CombatState is { } combatState)
        {
            foreach (Creature enemy in combatState.Enemies)
            {
                if (enemy.Monster is not { IntendsToAttack: true } monster)
                {
                    continue;
                }
                int totalDamage = monster.NextMove.Intents.OfType<AttackIntent>().Sum(intent => intent.GetTotalDamage(new[] { enemy }, monster.Creature));
                minimumIntentDamage = minimumIntentDamage is null ? totalDamage : Math.Min(minimumIntentDamage.Value, totalDamage);
            }
        }
        if (minimumIntentDamage is not null)
        {
            DynamicVars.Block.BaseValue = minimumIntentDamage.Value / 2;
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        }
    }
    //卡牌效果:
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
    //升级效果:伤害数值增加3
}
