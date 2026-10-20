using BCCE.BCCECode.Characters.Ironclad.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Ironclad.Cards;

// 聚力打击-失去1点生命，造成11点伤害，获得7点格挡。回合结束时本回合每使用一点能量，获得一点力量，下个回合结束时，失去等量力量。
public class ChargedStrike : IroncladCard
{
    public ChargedStrike() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    //定义卡牌基本属性：2能量，攻击，罕见，目标为任意敌人,图鉴可见
    {
    }
    public override bool GainsBlock => true;
    //定义卡牌是否获得格挡：是
    protected override HashSet<CardTag> CanonicalTags =>
    [
        CardTag.Strike
    ];
    //定义卡牌标签：打击，使本牌能被完美打击等统计打击数量的卡牌识别
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(1m),
        new DamageVar(11m, ValueProp.Move),
        new BlockVar(8m, ValueProp.Move)
    ];
    //定义可变参数:失去生命数值，初始为1;伤害数值，初始为11;格挡数值，初始为8
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
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<ChargedStrikePower>(choiceContext, Owner.Creature, 2m, Owner.Creature, this);
    }
    //卡牌效果:失去生命，对目标造成伤害，获得格挡，最后获得聚力打击能力
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars.Block.UpgradeValueBy(3m);
    }
    //升级效果:伤害数值增加4,格挡数值增加3
}
