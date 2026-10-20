using BCCE.BCCECode.Characters.Regent.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode.Characters.Regent.Cards;

// 潮汐-使一名敌人本回合失去8点力量，下回合获得4辉星。
public class Tide : RegentCard
{
    public Tide() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    //定义卡牌基本属性：2能量，技能，罕见，目标为任意敌人,图鉴可见
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("StrengthLoss", 8m),
        new StarsVar(4)
    ];
    //定义可变参数:敌人失去的力量数值，初始为8;下回合获得的辉星数量，初始为4
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<StarNextTurnPower>()
    ];
    //定义提示:提示力量与下回合辉星的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<TidePower>(choiceContext, cardPlay.Target, DynamicVars["StrengthLoss"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<StarNextTurnPower>(choiceContext, Owner.Creature, DynamicVars.Stars.BaseValue, Owner.Creature, this);
    }
    //卡牌效果:使目标在本回合失去力量，并为自己在下回合获得辉星
    protected override void OnUpgrade()
    {
        DynamicVars["StrengthLoss"].UpgradeValueBy(3m);
        DynamicVars.Stars.UpgradeValueBy(1m);
    }
    //升级效果:敌人失去的力量数值增加3
}
