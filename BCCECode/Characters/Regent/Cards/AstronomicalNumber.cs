using BCCE.BCCECode.Characters.Regent.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BCCE.BCCECode.Characters.Regent.Cards;

// 天文数字-你每花费3点辉星，对敌人造成的伤害提高10%。
public class AstronomicalNumber : RegentCard
{
    public AstronomicalNumber() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    //定义卡牌基本属性：2能量，能力，稀有，目标为自身,图鉴可见
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("StarsPerStack", 2m),
        new DynamicVar("DamageBonus", 10m)
    ];
    //定义可变参数:每档所需的辉星数量，初始为2;每档提高的伤害百分比，初始为10
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<AstronomicalNumberPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }
    //卡牌效果:获得天文数字能力
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    //升级效果:能量消耗减少1
}
