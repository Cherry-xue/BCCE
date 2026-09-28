using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Silent.Cards;

// 迅捷斩击-丢弃1张牌，对所有敌人造成9点伤害、给予1层虚弱。
public class SwiftStrike : SilentCard
{
    public SwiftStrike() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, true)
    //定义卡牌基本属性：0能量，攻击，罕见稀有度，目标为所有敌人,图鉴可见
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9, ValueProp.Move),
        new PowerVar<WeakPower>(1m)
    ];
    //定义可变参数:伤害数值,初始为9;虚弱层数，初始值为1
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>()
    ];
    //定义提示:提示虚弱的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).TargetingAllOpponents(CombatState).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        await PowerCmd.Apply<WeakPower>(choiceContext, CombatState.HittableEnemies, DynamicVars.Weak.BaseValue, Owner.Creature, this);
        CardModel cardModel = (await CardSelectCmd.FromHandForDiscard(choiceContext, Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1), null, this)).FirstOrDefault();
        if (cardModel != null)
        {
            await CardCmd.Discard(choiceContext, cardModel);
        }
    }
    //卡牌效果:
    protected override void OnUpgrade()
    {
        DynamicVars.Weak.UpgradeValueBy(1m);
    }
    //升级效果:虚弱层数增加1
}
