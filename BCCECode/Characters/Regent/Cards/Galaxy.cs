using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Regent.Cards;

// 银河-对所有敌人造成3点伤害4次，随机获得2个增益。
public class Galaxy : RegentCard
{
    public Galaxy() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies, true)
    //定义卡牌基本属性：1能量，攻击，稀有，目标为所有敌人,图鉴可见
    {
    }
    public override int CanonicalStarCost => 3;
    //定义卡牌的辉星消耗：3
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    //定义卡牌关键词：消耗
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new DynamicVar("BuffCount", 2m)
    ];
    //定义可变参数:伤害数值，初始为3;随机获得的增益数量，初始为2
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).TargetingAllOpponents(CombatState)
            .WithHitCount(4)
            .WithHitFx("vfx/vfx_starry_impact")
            .SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);
        await GrantRandomPowers(choiceContext);
    }
    //卡牌效果:对所有敌人造成多次伤害，然后随机获得若干个增益
    private async Task GrantRandomPowers(PlayerChoiceContext choiceContext)
    {
        //使用局内确定性随机源，保证多人联机各端抽到相同的结果
        Rng rng = Owner.RunState.Rng.CombatCardSelection;
        List<int> remaining = [];
        for (int i = 0; i < RandomPowerPool.All.Count; i++)
        {
            remaining.Add(i);
        }
        int count = (int)Math.Min(DynamicVars["BuffCount"].BaseValue, RandomPowerPool.All.Count);
        for (int i = 0; i < count; i++)
        {
            int picked = rng.NextInt(remaining.Count);
            await RandomPowerPool.All[remaining[picked]](choiceContext, Owner.Creature, Owner.Creature, this);
            remaining.RemoveAt(picked);
            await Cmd.Wait(0.3f);
        }
    }
    //随机从增益池中不放回地抽取若干个增益并依次施加，每个之间稍作停顿以播放动画
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
    //升级效果:移除消耗关键词
}
