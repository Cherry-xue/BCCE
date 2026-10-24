using BCCE.BCCECode.Characters.Silent.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode.Characters.Silent.Cards;

// 华尔兹夜曲-将3张墨影小刀加入所有玩家的手牌，本回合出牌者获得1点鬼祟。
public class WaltzNocturne : SilentCard
{
    public WaltzNocturne() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllAllies, true)
    //定义卡牌基本属性：2能量，技能，稀有，目标为所有玩家，图鉴可见
    {
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    //多人卡：仅在多人模式的卡池中出现

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new PowerVar<SneakyPower>(1m)
    ];
    //定义可变参数：每位玩家获得的小刀数量，初始为3；本回合获得的鬼祟层数，初始为1

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromCard<Shiv>(),
        .. HoverTipFactory.FromEnchantment<Inky>(),
        HoverTipFactory.FromPower<SneakyPower>()
    ];
    //定义提示：显示小刀、墨影附魔和鬼祟能力的相关信息

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var players = CombatState.GetTeammatesOf(Owner.Creature)
            .Where(creature => creature != null && creature.IsAlive && creature.IsPlayer)
            .Select(creature => creature.Player)
            .ToList();
        foreach (var player in players)
        {
            if (CombatManager.Instance.IsOverOrEnding)
            {
                return;
            }
            if (!player.Creature.IsAlive)
            {
                continue;
            }
            foreach (var shiv in await Shiv.CreateInHand(player, DynamicVars.Cards.IntValue, CombatState, Owner))
            {
                CardCmd.Enchant<Inky>(shiv, 1m);
            }
        }
        await PowerCmd.Apply<WaltzNocturnePower>(choiceContext, Owner.Creature,DynamicVars["SneakyPower"].BaseValue, Owner.Creature, this);
    }
    //卡牌效果：为每位存活玩家生成3张带墨影附魔的小刀，然后使出牌者在本回合获得鬼祟

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    //升级效果：能量消耗减少1
}
