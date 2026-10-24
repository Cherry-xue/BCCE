using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode.Characters.Ironclad.Cards;

// 火山-所有玩家抽4张牌，各自选择任意数量的手牌消耗；每消耗一张，对所有敌人施加1层易伤。
public class Volcano : IroncladCard
{
    public Volcano() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies, true)
    //定义卡牌基本属性：2能量，技能，罕见，目标为所有玩家，图鉴可见
    {
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    //多人卡：仅在多人模式的卡池中出现

    public override List<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Exhaust];
    //卡牌关键词：虚无、消耗

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(4),
        new PowerVar<VulnerablePower>(1m)
    ];
    //定义可变参数：抽牌数量，初始为4；每张消耗牌施加的易伤层数，初始为1

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>()
    ];
    //定义提示：显示易伤的相关信息

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var players = CombatState.GetTeammatesOf(Owner.Creature)
            .Where(creature => creature != null && creature.IsAlive && creature.IsPlayer)
            .Select(creature => creature.Player)
            .OrderBy(player => player == Owner)
            .ToList();
        //沿用原版抱团的玩家筛选；先派发队友的选择，再处理出牌者，避免自己的选择阻塞派发。
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
            await context.AssignTaskAndWaitForPauseOrCompletion(DrawAndExhaust(context, player));
        }
    }
    //卡牌效果：播放施法动画，为每位存活玩家分别派发抽牌、选择消耗手牌与施加群体易伤的流程

    private async Task DrawAndExhaust(PlayerChoiceContext choiceContext, Player player)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, player);
        if (CombatManager.Instance.IsOverOrEnding || !player.Creature.IsAlive)
        {
            return;
        }
        var selection = (await CardSelectCmd.FromHand(choiceContext, player,
            new CardSelectorPrefs(SelectionScreenPrompt, 0, int.MaxValue), null, this)).ToList();
        //每位玩家选择自己的手牌，允许不选；先固定选择结果，避免消耗触发抽牌后改变遍历内容。
        foreach (var card in selection)
        {
            if (CombatManager.Instance.IsOverOrEnding || !player.Creature.IsAlive)
            {
                return;
            }
            if (card.Pile != PileType.Hand.GetPile(player))
            {
                continue;
            }
            var result = await CardCmd.Exhaust(choiceContext, card);
            if (result is { success: true, targetPile: PileType.Exhaust })
            {
                await PowerCmd.Apply<VulnerablePower>(choiceContext, CombatState.HittableEnemies,
                    DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
            }
        }
    }
    //单个玩家的结算：抽牌后任选手牌消耗，每成功消耗一张就施加一次群体易伤，不包含火山自身的消耗

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
    //升级效果：抽牌数量增加1
}
