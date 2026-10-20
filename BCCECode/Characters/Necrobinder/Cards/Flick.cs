using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Necrobinder.Cards;

// 弹指-奥斯提造成8点伤害，抽1张牌。
public class Flick : NecrobinderCard
{
    public Flick() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    //定义卡牌基本属性：1能量，攻击，常见，目标为任意敌人,图鉴可见
    {
    }
    protected override HashSet<CardTag> CanonicalTags =>
    [
        CardTag.OstyAttack
    ];
    //定义卡牌标签：奥斯提攻击，使本牌能被钙化、响骨等依据奥斯提攻击次数结算的卡牌识别
    protected override bool ShouldGlowRedInternal => Owner.IsOstyMissing;
    //定义红光提示：奥斯提不在场或已死亡时，本牌发出红光提示
    public override List<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Ethereal
    ];
    //卡牌关键词：虚无
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new OstyDamageVar(8m, ValueProp.Move),
        new CardsVar(1)
    ];
    //定义可变参数:奥斯提伤害数值，初始为8;抽牌数量，初始值为1
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        if (!Osty.CheckMissingWithAnim(Owner))
        {
            await DamageCmd.Attack(DynamicVars.OstyDamage.BaseValue).FromOsty(Owner.Osty, this, cardPlay).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
                .Execute(choiceContext);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }
    }
    //卡牌效果:奥斯提对目标造成伤害，然后抽1张牌
    protected override void OnUpgrade()
    {
        DynamicVars.OstyDamage.UpgradeValueBy(2m);
    }
    //升级效果:奥斯提伤害数值增加2
}
