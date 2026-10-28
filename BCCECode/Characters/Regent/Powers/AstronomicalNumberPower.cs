using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Regent.Powers;

// 天文数字-你每花费2点辉星，对敌人造成的伤害提高10%。
public sealed class AstronomicalNumberPower : RegentPower
{
    private class Data
    {
        public int starsSpent;
    }
    //定义内部数据类，记录本场战斗累计花费的辉星数量
    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益
    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    //定义实例类型：独立实例
    public override int DisplayAmount => (int)DynamicVars["CurrentBonus"].BaseValue;
    //定义显示数值:当前已经提高的伤害百分比
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("StarsPerStack", 2m),
        new DynamicVar("DamageBonus", 10m),
        new DynamicVar("RemainingStars", 2m),
        new DynamicVar("CurrentBonus", 0m)
    ];
    //定义可变参数:每档所需的辉星数量，初始为2;每档提高的伤害百分比，初始为10;距离下次提高伤害还需花费的辉星数量;当前已经提高的伤害百分比
    protected override object InitInternalData()
    {
        return new Data();
    }
    //初始化内部数据：返回一个新的Data对象
    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player == Owner.Player && cardPlay.IsLastInSeries && cardPlay.Resources.StarsSpent > 0)
        {
            GetInternalData<Data>().starsSpent += cardPlay.Resources.StarsSpent;
            RefreshVars();
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }
    //在整张牌结算后触发：累计实际花费的辉星并刷新增伤；重放序列只在最后一次计数，本次花费不增强该牌自身的伤害
    private void RefreshVars()
    {
        int perStack = (int)DynamicVars["StarsPerStack"].BaseValue;
        int stacks = GetInternalData<Data>().starsSpent / perStack;
        DynamicVars["RemainingStars"].BaseValue = perStack - GetInternalData<Data>().starsSpent % perStack;
        DynamicVars["CurrentBonus"].BaseValue = stacks * DynamicVars["DamageBonus"].BaseValue;
    }
    //刷新两个显示用的变量：距离下次提高伤害还需花费的辉星数量，以及当前已经提高的伤害百分比（写法参照聚力打击）
    public override decimal ModifyDamageMultiplicative(Creature target, decimal amount, ValueProp props, Creature dealer, CardModel cardSource, CardPlay cardPlay)
    {
        if (!props.IsPoweredAttack() || cardSource == null)
        {
            return 1m;
        }
        if (dealer != Owner && !Owner.Pets.Contains(dealer))
        {
            return 1m;
        }
        if (target == null || !target.IsEnemy)
        {
            return 1m;
        }
        int stacks = GetInternalData<Data>().starsSpent / (int)DynamicVars["StarsPerStack"].BaseValue;
        return 1m + stacks * DynamicVars["DamageBonus"].BaseValue / 100m;
    }
    //在计算伤害倍率时触发：只有自己通过卡牌对敌人造成的攻击伤害，才按累计花费的辉星提高伤害
}
