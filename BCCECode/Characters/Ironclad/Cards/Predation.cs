using BCCE.BCCECode.Characters.Ironclad.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode.Characters.Ironclad.Cards;

// 弱肉强食-每当你获得力量时，使随机一名敌人失去等量力量。
public class Predation : IroncladCard
{
    public Predation() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    //定义卡牌基本属性：1能量，能力，稀有，目标为自身,图鉴可见
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PredationPower>(1m)
    ];
    //定义可变参数:弱肉强食能力的持续回合数，初始为1
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    //定义提示:提示力量的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<PredationPower>(choiceContext, Owner.Creature, DynamicVars["PredationPower"].BaseValue, Owner.Creature, this);
    }
    //卡牌效果:获得弱肉强食能力
    protected override void OnUpgrade()
    {
        DynamicVars["PredationPower"].UpgradeValueBy(1m);
    }
    //升级效果:持续回合数增加1
}
