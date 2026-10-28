using BCCE.BCCECode.Characters.Ironclad.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Ironclad.Cards;

// 聚力打击-失去1点生命，造成3点伤害；回合结束时每花费1点能量获得2点力量，连续聚力可以保留并累积力量。
public class ChargedStrike : IroncladCard
{
    public ChargedStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    //定义卡牌基本属性：1能量，攻击，普通，目标为任意敌人，图鉴可见
    {
    }
    protected override HashSet<CardTag> CanonicalTags =>
    [
        CardTag.Strike
    ];
    //定义卡牌标签：打击，使本牌能被完美打击等统计打击数量的卡牌识别
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(1m),
        new DamageVar(3m, ValueProp.Move),
        new DynamicVar("ChargedStrikePower", 1m)
    ];
    //定义可变参数：失去生命数值，初始为1;伤害数值，初始为3;聚力打击待结算能力的初始值为1
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    //定义提示:提示力量的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
        await PowerCmd.Apply<ChargedStrikePower>(choiceContext, Owner.Creature, DynamicVars["ChargedStrikePower"].BaseValue, Owner.Creature, this);
    }
    //卡牌效果：失去1点生命，对目标造成伤害，最后获得独立的聚力打击待结算能力
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["ChargedStrikePower"].UpgradeValueBy(1m);
    }
    //升级效果：伤害数值增加2，聚力打击待结算能力增加1
}
