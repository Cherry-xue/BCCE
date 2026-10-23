using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace BCCE.BCCECode.Characters.Necrobinder.Powers;

// 万圣夜-下两次奥斯提在敌方回合受到致命伤害时，依次以其最大生命值的50%和25%的血量复活，不改变生命上限。
public sealed class HalloweenPower : NecrobinderPower
{
    private class Data
    {
        public int revivesUsed;
    }
    //定义内部数据类，记录已经复活的次数
    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益
    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器
    public override int DisplayAmount => (int)Amount - GetInternalData<Data>().revivesUsed;
    //定义显示数值:剩余的复活次数
    protected override object InitInternalData()
    {
        return new Data();
    }
    //初始化内部数据：返回一个新的Data对象
    public override bool ShouldDie(Creature creature)
    {
        if (!CanRevive(creature))
        {
            return true;
        }
        return false;
    }
    //在生物将要死亡时触发：符合条件则返回 false 阻止死亡（写法参照原版瓶中精灵）
    public override async Task AfterPreventingDeath(Creature creature)
    {
        Data data = GetInternalData<Data>();
        decimal targetHp = Math.Floor(creature.MaxHp * (data.revivesUsed == 0 ? 0.5m : 0.25m));
        Flash();
        await CreatureCmd.Heal(creature, targetHp - creature.CurrentHp);
        data.revivesUsed++;
        InvokeDisplayAmountChanged();
        if (data.revivesUsed >= (int)Amount || creature.CurrentHp <= 0)
        {
            await PowerCmd.Remove(this);
        }
    }
    //在阻止死亡后触发：第一次将其回复到最大生命值的50%，第二次回复到25%，用尽后移除本能力
    //注意：治疗按"补差额"计算（写法参照原版幻象），并且若治疗没有生效则立刻移除本能力，避免死亡被反复阻止而触发递归保护
    private bool CanRevive(Creature creature)
    {
        if (GetInternalData<Data>().revivesUsed >= (int)Amount)
        {
            return false;
        }
        if (Owner.Player.Osty == null || creature != Owner.Player.Osty)
        {
            return false;
        }
        return CombatState.CurrentSide == CombatSide.Enemy;
    }
    //判断能否复活：次数未用尽、死者是奥斯提、且当前处于敌方回合
    public void Refresh(int amount)
    {
        GetInternalData<Data>().revivesUsed = 0;
        SetAmount(amount);
        InvokeDisplayAmountChanged();
        Flash();
    }
    //刷新本能力：将层数直接设为指定数值并清空已复活的次数，而不是在原有层数上继续叠加
}
