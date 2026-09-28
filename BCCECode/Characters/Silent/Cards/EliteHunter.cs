using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace BCCE.BCCECode.Characters.Silent.Cards;

// 精英猎手-获得2点力量、2点敏捷，你的下1张攻击牌可以免费打出。若本场战斗为精英或boss，本卡牌会自动打出,并额外获得1点力量和敏捷，抽1张牌。
public class EliteHunter : SilentCard
{
    public EliteHunter() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    //定义卡牌基本属性：3能量，能力，稀有，目标为自己,图鉴可见
    {
    }
    private bool isElite = false;
    //定义一个布尔变量isElite，初始值为false，用于判断当前房间是否为精英或boss
    public override List<CardKeyword> CanonicalKeywords =>
    [
        BCCEKeywords.EliteImbued
    ];
    //卡牌关键词:精英注能
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(2m),
        new PowerVar<DexterityPower>(2m),
        new DynamicVar("EliteStrengthPower", 1m),
        new DynamicVar("EliteDexterityPower", 1m),
        new CardsVar(1)
    ];
    //定义可变参数:力量数值，初始值为2;敏捷数值，初始值为2;精英力量数值，初始值为1;精英敏捷数值，初始值为1;卡牌数量，初始值为1
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>()
    ];
    //定义提示:提示力量和敏捷的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature, DynamicVars.Dexterity.BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars.Strength.BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<FreeAttackPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        // 如果是精英或boss
        if (isElite)
        {
            await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature, DynamicVars["EliteDexterityPower"].BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars["EliteStrengthPower"].BaseValue, Owner.Creature, this);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }
    }
    //卡牌效果:
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner.Creature) && Owner.PlayerCombatState.TurnNumber <= 1)
        {
            AbstractRoom currentRoom = combatState.RunState.CurrentRoom;
            if (currentRoom != null)
            {
                if (currentRoom.RoomType == RoomType.Elite || currentRoom.RoomType == RoomType.Boss)
                {
                    isElite = true;
                    await CardCmd.AutoPlay(new ThrowingPlayerChoiceContext(), this, null);
                }
            }
        }
    }
    //在回合开始后触发的事件中，检查当前回合是否为玩家的回合，如果是，则根据当前房间类型判断是否为精英或boss，如果是，则自动打出该卡牌
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    //升级效果:减少1点能量消耗
}
