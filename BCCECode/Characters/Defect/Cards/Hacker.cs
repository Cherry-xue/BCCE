using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace BCCE.BCCECode.Characters.Defect.Cards;

// 黑客-所有玩家生成1个黑暗充能球，并立即触发1次新生成球的被动。
public class Hacker : DefectCard
{
    public Hacker() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllAllies, true)
    //定义卡牌基本属性：2能量，技能，稀有，目标为所有玩家，图鉴可见
    {
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    //多人卡：仅在多人模式的卡池中出现

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<DarkOrb>()
    ];
    //定义提示：显示生成充能球与黑暗充能球的相关信息

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var players = CombatState.GetTeammatesOf(Owner.Creature)
            .Where(creature => creature != null && creature.IsAlive && creature.IsPlayer)
            .Select(creature => creature.Player)
            .OrderBy(player => player == Owner)
            .ToList();
        foreach (Player player in players)
        {
            if (CombatManager.Instance.IsOverOrEnding)
            {
                return;
            }
            if (!player.Creature.IsAlive)
            {
                continue;
            }
            var context = new BranchingPlayerChoiceContext(this, LocalContext.NetId.Value, GameActionType.Combat, choiceContext);
            await context.AssignTaskAndWaitForPauseOrCompletion(ChannelAndTrigger(context, player));
        }
    }
    //卡牌效果：为每位存活玩家分别派发生成黑暗球并触发其被动的流程，生成过程中的选择沿用原版多人分支上下文

    private async Task ChannelAndTrigger(PlayerChoiceContext choiceContext, Player player)
    {
        var orb = ModelDb.Orb<DarkOrb>().ToMutable();
        await OrbCmd.Channel(choiceContext, orb, player);
        if (player.Creature.IsAlive && !CombatManager.Instance.IsOverOrEnding
            && player.PlayerCombatState.OrbQueue.Orbs.Contains(orb))
        {
            await OrbCmd.Passive(choiceContext, orb, null);
        }
    }
    //单个玩家的结算：由原版处理球位与生成，仅当本次新生成的黑暗球仍在球栏中时触发一次被动

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    //升级效果：能量消耗减少1
}
