using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Silent.Cards;

// 金蛇狂咬-随机对敌人造成2点伤害8次，每次攻击给予等量于造成伤害的中毒。
public class GoldenSnakeBites : SilentCard
{
    public GoldenSnakeBites() : base(2, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy, true)
    //定义卡牌基本属性：2能量，攻击，稀有稀有度，目标为所有敌人,图鉴可见
    {
    }
    public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];
    //卡牌关键词：保留
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2, ValueProp.Move),
        new RepeatVar(8)
    ];
    //定义可变参数:伤害数值,初始为2;重复次数，初始值为8
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>()
    ];
    //定义提示:提示中毒的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for (int i = 0; i < DynamicVars.Repeat.IntValue; i++)
        {
            Creature enemy = Owner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
            if (enemy == null)
            {
                continue;
            }
            AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
            .Targeting(enemy)
            .WithHitFx("vfx/vfx_attack_slash", null, "blunt_attack.mp3")
            .Execute(choiceContext);
            await PowerCmd.Apply<PoisonPower>(choiceContext, enemy, attackCommand.Results.SelectMany((List<DamageResult> r) => r).Sum((DamageResult r) => r.TotalDamage), Owner.Creature, this);
        }    
    }
    //卡牌效果:
    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1m);
    }
    //升级效果:重复次数增加1
}
