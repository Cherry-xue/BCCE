using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Regent.Powers;

// 不明飞行物-任意玩家打出一张无色牌时，使随机一名敌人失去等同于能力层数的生命。
public sealed class UnidentifiedFlyingObjectPower : RegentPower
{
    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益

    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器，重复施加时累加每次触发的生命流失数值

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!cardPlay.Card.VisualCardPool.IsColorless || cardPlay.Card.Owner.Creature.Side != Owner.Side
            || CombatManager.Instance.IsOverOrEnding || CombatState.HittableEnemies.Count == 0)
        {
            return;
        }
        var enemy = Owner.Player.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        if (enemy != null)
        {
            Flash();
            await CreatureCmd.Damage(choiceContext, new[] { enemy }, Amount,
                ValueProp.Unblockable | ValueProp.Unpowered, null, null, null);
        }
    }
    //在任意玩家打出无色牌后触发：沿用原版纠缠的随机目标与生命流失结算，使随机敌人失去等同于层数的生命
}
