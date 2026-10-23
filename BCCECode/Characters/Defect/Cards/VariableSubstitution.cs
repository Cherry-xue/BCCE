using BCCE.BCCECode.Characters.Defect.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode.Characters.Defect.Cards;

// 变量置换-你每生成5个充能球，获得2点集中。每有4点集中，在你的回合开始时，额外获得1点能量。
public class VariableSubstitution : DefectCard
{
    public VariableSubstitution() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    //定义卡牌基本属性：2能量，能力，稀有，目标为自身,图鉴可见
    {
    }
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Innate
    ];
    //定义卡牌关键词：固有
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("OrbsPerStack", 5m),
        new DynamicVar("FocusGain", 2m),
        new DynamicVar("FocusPerEnergy", 4m)
    ];
    //定义可变参数:每次获得集中所需的充能球数量，初始为5;每次获得的集中层数，初始为2;每点额外能量所需的集中层数，初始为4
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<FocusPower>()
    ];
    //定义提示:提示集中的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<VariableSubstitutionPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }
    //卡牌效果:获得变量置换能力
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    //升级效果:能量消耗减少1
}
