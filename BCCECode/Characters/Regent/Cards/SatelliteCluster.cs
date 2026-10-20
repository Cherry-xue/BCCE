using BCCE.BCCECode.Characters.Regent.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Regent.Cards;

// 卫星集群-打出此牌后，你在本回合每获得或失去1点辉星，铸造6、获得3点格挡。
public class SatelliteCluster : RegentCard
{
    public SatelliteCluster() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    //定义卡牌基本属性：1能量，技能，稀有，目标为自身,图鉴可见
    {
    }
    public override int CanonicalStarCost => 2;
    //定义卡牌的辉星消耗：2
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SatelliteClusterPower>(6m),
        new BlockVar(3m, ValueProp.Move)
    ];
    //定义可变参数:卫星集群能力的层数，初始为6;获得的格挡数值，初始为层数的一半
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        .. HoverTipFactory.FromForge(),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];
    //定义提示:提示铸造与格挡的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SatelliteClusterPower>(choiceContext, Owner.Creature, DynamicVars["SatelliteClusterPower"].BaseValue, Owner.Creature, this);
    }
    //卡牌效果:抽牌，然后获得卫星集群能力
    protected override void OnUpgrade()
    {
        DynamicVars["SatelliteClusterPower"].UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(1m);
        AddKeyword(CardKeyword.Retain);
    }
    //升级效果:卫星集群能力的层数增加2，格挡数值增加1,添加保留关键词
}
