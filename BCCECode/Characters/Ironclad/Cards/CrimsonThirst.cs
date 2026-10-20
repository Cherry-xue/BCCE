using BCCE.BCCECode.Characters.Ironclad.Powers;
using BCCE.BCCECode.Patch;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

namespace BCCE.BCCECode.Characters.Ironclad.Cards;

// 猩红渴求-本场战斗结束时燃烧之血/黑暗之血不提供生命恢复，每当有一名不是“爪牙”的敌人死亡，获得1点最大生命值。
public class CrimsonThirst : IroncladCard
{
    public CrimsonThirst() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    //定义卡牌基本属性：1能量，能力，罕见，目标为自身,图鉴可见
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<CrimsonThirstPower>(1m)
    ];
    //定义可变参数:每击杀一名敌人获得的最大生命值，初始为1
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<CrimsonThirstPower>()
    ];
    //定义提示:提示猩红渴求的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        if (CombatState.RunState.CurrentRoom is CombatRoom room)
        {
            BloodHealSuppressor.Mark(Owner, room);
        }
        await PowerCmd.Apply<CrimsonThirstPower>(choiceContext, Owner.Creature, DynamicVars["CrimsonThirstPower"].BaseValue, Owner.Creature, this);
    }
    //卡牌效果:记录本场战斗需要屏蔽燃烧之血/黑暗之血的治疗，然后获得猩红渴求能力
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    //升级效果:减少1点能量消耗
}
