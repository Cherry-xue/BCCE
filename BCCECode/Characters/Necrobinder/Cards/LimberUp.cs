using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode.Characters.Necrobinder.Cards;

// 舒活筋骨-获得4点活力，奥斯提回复4点生命。
public class LimberUp : NecrobinderCard
{
    public LimberUp() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    //定义卡牌基本属性：1能量，技能，常见，目标为自身,图鉴可见
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VigorPower>(4m),
        new HealVar(6m)
    ];
    //定义可变参数:活力数值，初始为4;治疗数值，初始为6
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<VigorPower>()
    ];
    //定义提示:提示活力的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, DynamicVars["VigorPower"].BaseValue, Owner.Creature, this);
        if (Owner.Osty is { IsAlive: true } osty)
        {
            await CreatureCmd.Heal(osty, DynamicVars.Heal.BaseValue);
        }
    }
    //卡牌效果:获得活力；若奥斯提在场且存活，则为其回复生命。回复只作用于当前生命，不会提升奥斯提的最大生命
    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(4m);
        DynamicVars["VigorPower"].UpgradeValueBy(3m);
    }//升级效果:治疗数值增加4,活力数值增加3
}
