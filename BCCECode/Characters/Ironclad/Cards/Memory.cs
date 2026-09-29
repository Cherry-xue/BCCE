using BCCE.BCCECode.Characters.Ironclad.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Ironclad.Cards;

// 回忆-获得15点格挡,将4张伤口加入手牌,回合结束时,消耗所有非消耗堆的状态牌。
public class Memory : IroncladCard
{
    public Memory() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    //定义卡牌基本属性：3能量，技能，稀有，目标为自身,图鉴可见
    {
    }
    public override bool GainsBlock => true;
    //定义卡牌是否获得格挡：是
    public override List<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Exhaust,
        CardKeyword.Eternal
    ];
    //卡牌关键词：消耗,永恒
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(15, ValueProp.Move),
        new CardsVar(4)
    ];
    //定义可变参数:格挡数值，初始值为15;卡牌数量，初始值为4;  
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromCard<Wound>()
    ];
    //定义额外的悬停提示：显示伤口的悬停提示
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        for (int i = 0; i < DynamicVars.Cards.BaseValue; i++)
        {
            CardModel card = CombatState.CreateCard<Wound>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
        }
        await PowerCmd.Apply<MemoryPower>(choiceContext, Owner.Creature, 1, Owner.Creature, null);
    }
    //卡牌效果:
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
    //升级效果:添加保留关键词
}
