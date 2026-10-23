using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode.Characters.Defect.Powers;

// 变量置换-你每生成5个充能球，获得2点集中。每有4点集中，在你的回合开始时，额外获得1点能量。
public sealed class VariableSubstitutionPower : DefectPower
{
    private class Data
    {
        public int orbsChanneled;
    }
    //定义内部数据类，记录本场战斗累计生成的充能球数量
    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益
    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    //定义实例类型：独立实例
    public override int DisplayAmount => (int)DynamicVars["OrbsPerStack"].BaseValue - GetInternalData<Data>().orbsChanneled % (int)DynamicVars["OrbsPerStack"].BaseValue;
    //定义显示数值:距离下次获得集中还需要生成的充能球数量
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("OrbsPerStack", 5m),
        new DynamicVar("FocusGain", 2m),
        new DynamicVar("FocusPerEnergy", 4m),
        new DynamicVar("PreviewEnergy", 0m)
    ];
    //定义可变参数:每次获得集中所需的充能球数量，初始为5;每次获得的集中层数，初始为2;每点额外能量所需的集中层数，初始为4;当前集中能在回合开始时提供的额外能量
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<FocusPower>()
    ];
    //定义额外的悬停提示：显示集中的相关信息
    protected override object InitInternalData()
    {
        return new Data();
    }
    //初始化内部数据：返回一个新的Data对象
    public override Task BeforeApplied(Creature target, decimal amount, Creature applier, CardModel cardSource)
    {
        RefreshPreviewEnergy(target);
        return Task.CompletedTask;
    }
    //在施加时触发：先按当前集中刷新一次额外能量的预览数值
    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature applier, CardModel cardSource)
    {
        if (power is FocusPower && power.Owner == Owner)
        {
            RefreshPreviewEnergy(Owner);
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }
    //在能力层数变化后触发：集中发生变化时刷新额外能量的预览数值
    private void RefreshPreviewEnergy(Creature creature)
    {
        FocusPower focus = creature.GetPower<FocusPower>();
        DynamicVars["PreviewEnergy"].BaseValue = focus == null ? 0m : Math.Floor(focus.Amount / DynamicVars["FocusPerEnergy"].BaseValue);
    }
    //按当前集中的层数向下取整，计算回合开始时能额外获得的能量数量供描述显示
    public override async Task AfterOrbChanneled(PlayerChoiceContext choiceContext, Player player, OrbModel orb)
    {
        if (player != Owner.Player)
        {
            return;
        }
        Data data = GetInternalData<Data>();
        data.orbsChanneled++;
        if (data.orbsChanneled % (int)DynamicVars["OrbsPerStack"].BaseValue == 0)
        {
            Flash();
            await PowerCmd.Apply<FocusPower>(choiceContext, Owner, DynamicVars["FocusGain"].BaseValue, Owner, null);
        }
        InvokeDisplayAmountChanged();
    }
    //在生成充能球后触发：累计生成的充能球数量，每满一定数量就获得集中，最后更新显示数值
    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }
        int energy = (int)DynamicVars["PreviewEnergy"].BaseValue;
        if (energy > 0)
        {
            Flash();
            await PlayerCmd.GainEnergy(energy, player);
        }
    }
    //在回合开始时触发：按当前集中的层数向下取整，额外获得相应数量的能量
}
