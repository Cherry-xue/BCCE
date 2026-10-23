using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Defect.Cards;

// 十万伏特-造成3点伤害3次，每次命中若未被格挡，则每次生成2个闪电充能球。
public class HundredThousandVolts : DefectCard
{
    public HundredThousandVolts() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    //定义卡牌基本属性：3能量，攻击，稀有，目标为任意敌人,图鉴可见
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new RepeatVar(3),
        new DynamicVar("Orbs", 2m)
    ];
    //定义可变参数:伤害数值，初始为3;攻击次数，初始为3;每次生成的闪电充能球数量，初始为2
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromOrb<LightningOrb>()
    ];
    //定义提示:提示闪电充能球的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .WithHitFx("vfx/vfx_attack_slash", null, "slash_attack.mp3")
            .Execute(choiceContext);
        foreach (DamageResult result in attackCommand.Results.SelectMany((List<DamageResult> r) => r))
        {
            if (result.WasFullyBlocked)
            {
                continue;
            }
            for (int i = 0; i < DynamicVars["Orbs"].IntValue; i++)
            {
                await OrbCmd.Channel<LightningOrb>(choiceContext, Owner);
            }
        }
    }
    //卡牌效果:造成多次伤害，每次未被完全格挡的命中都会生成闪电充能球
    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1m);
    }
    //升级效果:攻击次数增加1
}
