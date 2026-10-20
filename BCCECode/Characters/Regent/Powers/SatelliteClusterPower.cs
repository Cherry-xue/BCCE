using BCCE.BCCECode.Characters.Regent.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Regent.Powers;

// 卫星集群-本回合每获得或失去1点辉星，铸造、获得格挡
public sealed class SatelliteClusterPower : RegentPower
{
    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益
    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(3m, ValueProp.Move)
    ];
    //定义可变参数:获得的格挡数值，初始为层数的一半
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        .. HoverTipFactory.FromForge(),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];
    //定义额外的悬停提示：显示铸造与格挡的相关信息
    public override Task BeforeApplied(Creature target, decimal amount, Creature applier, CardModel cardSource)
    {
        DynamicVars.Block.BaseValue = amount / 2m;
        return Task.CompletedTask;
    }
    //在施加时触发：按能力层数设置格挡数值，供卡面与能力描述显示
    public override async Task AfterStarsGained(int amount, Player gainer)
    {
        if (amount > 0 && gainer == Owner.Player)
        {
            await Trigger(amount);
        }
    }
    //在获得辉星后触发：按获得的辉星数量结算铸造与格挡
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card is SatelliteCluster || !cardPlay.IsLastInSeries)
        {
            return;
        }
        if (cardPlay.Resources.StarsSpent > 0 && cardPlay.Card.Owner == Owner.Player)
        {
            await Trigger(cardPlay.Resources.StarsSpent);
        }
    }
    //在卡牌打出后触发：若该牌消耗了辉星则结算；本卡自身消耗的辉星不参与结算
    private async Task Trigger(int stars)
    {
        Flash();
        DynamicVars.Block.BaseValue = Amount / 2m;
        await ForgeCmd.Forge(Amount * stars, Owner.Player, this);
        await CreatureCmd.GainBlock(Owner, DynamicVars.Block.BaseValue * stars, DynamicVars.Block.Props, null);
    }
    //结算：按辉星数量铸造等同于能力层数的数值，并获得等同于能力层数一半的格挡
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
    //在回合结束前触发：本能力只在本回合生效
}
