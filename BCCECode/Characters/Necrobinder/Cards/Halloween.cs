using BCCE.BCCECode.Characters.Necrobinder.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BCCE.BCCECode.Characters.Necrobinder.Cards;

// 万圣夜-下两次奥斯提在敌方回合受到致命伤害时，依次以其最大生命值的50%和25%的血量复活。
public class Halloween : NecrobinderCard
{
    public Halloween() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    //定义卡牌基本属性：2能量，技能，稀有，目标为自身,图鉴可见
    {
    }
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    //定义卡牌关键词：消耗
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<HalloweenPower>(2m)
    ];
    //定义可变参数:万圣夜能力的复活次数，初始为2
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];
    //定义提示:提示消耗的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        HalloweenPower halloween = Owner.Creature.GetPower<HalloweenPower>();
        if (halloween == null)
        {
            await PowerCmd.Apply<HalloweenPower>(choiceContext, Owner.Creature, DynamicVars["HalloweenPower"].BaseValue, Owner.Creature, this);
            return;
        }
        halloween.Refresh(DynamicVars["HalloweenPower"].IntValue);
    }
    //卡牌效果:获得万圣夜能力；若已经拥有该能力，则不再叠加层数，而是把它刷新为新的层数并重置已复活的次数，避免层数超过两次导致复活次数与描述不符
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
    //升级效果:添加保留关键词
}
