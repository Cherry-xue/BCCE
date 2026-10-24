using BCCE.BCCECode.Characters.Necrobinder.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BCCE.BCCECode.Characters.Necrobinder.Cards;

// 指挥-每有1名其他玩家，在你的回合开始时额外召唤1。
public class Command : NecrobinderCard
{
    public Command() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    //定义卡牌基本属性：1能量，能力，罕见，目标为自身，图鉴可见
    {
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    //多人卡：仅在多人模式的卡池中出现

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SummonVar(1m)
    ];
    //定义可变参数：每名其他玩家提供的额外召唤值，初始为1

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.SummonDynamic, DynamicVars.Summon)
    ];
    //定义提示：显示召唤的相关信息

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<CommandPower>(choiceContext, Owner.Creature, DynamicVars.Summon.BaseValue, Owner.Creature, this);
    }
    //卡牌效果：为自己施加指挥能力，后续每回合开始时按其他玩家人数额外召唤

    protected override void OnUpgrade()
    {
        DynamicVars.Summon.UpgradeValueBy(1m);
    }
    //升级效果：每名其他玩家提供的额外召唤值增加1
}
