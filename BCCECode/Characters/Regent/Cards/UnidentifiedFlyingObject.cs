using BCCE.BCCECode.Characters.Regent.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BCCE.BCCECode.Characters.Regent.Cards;

// 不明飞行物-所有玩家从3张随机无色牌中选1张放入弃牌堆；任意玩家打出无色牌时，使随机敌人失去5点生命。
public class UnidentifiedFlyingObject : RegentCard
{
    public UnidentifiedFlyingObject() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.AllAllies, true)
    //定义卡牌基本属性：1能量，能力，罕见，目标为所有玩家，图鉴可见
    {
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    //多人卡：仅在多人模式的卡池中出现

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new HpLossVar(5m)
    ];
    //定义可变参数：随机候选无色牌数量，初始为3；每次触发的生命流失数值，初始为5

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<UnidentifiedFlyingObjectPower>()
    ];
    //定义提示：显示不明飞行物能力的相关信息

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<UnidentifiedFlyingObjectPower>(choiceContext, Owner.Creature,
            DynamicVars.HpLoss.BaseValue, Owner.Creature, this);
        var choices = CombatState.GetTeammatesOf(Owner.Creature)
            .Where(creature => creature != null && creature.IsAlive && creature.IsPlayer)
            .Select(creature => creature.Player)
            .Select(player => (Player: player, Cards: CardFactory.GetDistinctForCombat(player,
                ModelDb.CardPool<ColorlessCardPool>().GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint),
                DynamicVars.Cards.IntValue, player.RunState.Rng.CombatCardGeneration).ToList()))
            .OrderBy(choice => choice.Player == Owner)
            .ToList();
        //参照类星体，为各玩家生成不重复的候选牌；先固定所有随机结果，再派发队友与出牌者的选择。
        foreach (var choice in choices)
        {
            if (CombatManager.Instance.IsOverOrEnding)
            {
                return;
            }
            if (!choice.Player.Creature.IsAlive || choice.Cards.Count == 0)
            {
                continue;
            }
            var context = new BranchingPlayerChoiceContext(this, LocalContext.NetId.Value, GameActionType.Combat, choiceContext);
            await context.AssignTaskAndWaitForPauseOrCompletion(ChooseForPlayer(context, choice.Player, choice.Cards));
        }
    }
    //卡牌效果：获得监听所有玩家无色牌的不明飞行物能力，再为每位存活玩家分别派发随机无色牌三选一

    private async Task ChooseForPlayer(PlayerChoiceContext choiceContext, Player player, List<CardModel> cards)
    {
        var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, player, canSkip: false);
        if (selected != null && player.Creature.IsAlive && !CombatManager.Instance.IsOverOrEnding)
        {
            await CardPileCmd.AddGeneratedCardToCombat(selected, PileType.Discard, Owner);
        }
    }
    //单个玩家的选择：必须从候选无色牌中选择1张，将其作为战斗生成牌放入该玩家的弃牌堆

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
    //升级效果：添加固有关键词
}
