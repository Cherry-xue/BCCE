using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BCCE.BCCECode.Characters.Ironclad.Powers;

// 猩红渴求-本场战斗结束时燃烧之血/黑暗之血不提供生命恢复，每当有一名不是“爪牙”的敌人死亡，获得1点最大生命值
public sealed class CrimsonThirstPower : IroncladPower
{
    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益
    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器
    public override int DisplayAmount => Amount;
    //定义显示数值:每有一名敌人死亡时获得的最大生命值
    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || creature.Side == Owner.Side || !creature.Powers.All(p => p.ShouldOwnerDeathTriggerFatal()))
        {
            return;
        }
        Flash();
        await CreatureCmd.GainMaxHp(Owner, Amount);
    }
    //在生物死亡后触发：若死亡的是敌方且不为爪牙，则获得最大生命值
}
