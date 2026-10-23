using System.Collections.Generic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BCCE.BCCECode.Characters.Necrobinder.Cards;

// 彼岸呼号-召唤1。奥斯提造成10点伤害。额外造成等量于奥斯提最大生命值的伤害。如果这张牌杀死了一名不为爪牙的敌人，则使召唤的数字在本局游戏永久增加该敌人最大生命值的10%。
public class CallFromBeyond : NecrobinderCard
{
    private const int _baseSummon = 1;
    //定义本牌召唤数值的基准，初始为1
    private int _increasedSummon;
    //定义一个私有字段_increasedSummon，用于记录这张牌累计增加的召唤数值
    public CallFromBeyond() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    //定义卡牌基本属性：2能量，攻击，稀有，目标为任意敌人,图鉴可见
    {
    }
    protected override HashSet<CardTag> CanonicalTags =>
    [
        CardTag.OstyAttack
    ];
    //定义卡牌标签：奥斯提攻击，使本牌能被钙化、响骨等依据奥斯提攻击次数结算的卡牌识别
    [SavedProperty]
    public int IncreasedSummon
    {
        get
        {
            return _increasedSummon;
        }
        set
        {
            AssertMutable();
            _increasedSummon = value;
            DynamicVars.Summon.BaseValue = _baseSummon + _increasedSummon;
        }
    }
    //定义可保存属性：累计增加的召唤数值，写入时同步更新召唤变量以便卡面显示
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    //定义卡牌关键词：消耗
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(10m),
        new ExtraDamageVar(1m).FromOsty(),
        new CalculatedDamageVar(ValueProp.Move).FromOsty().WithMultiplier(delegate (CardModel card, Creature _)
        {
            Creature osty = card.Owner.Osty;
            return osty != null && osty.IsAlive ? osty.CurrentHp : 0;
        }),
        new SummonVar(_baseSummon),
        new DynamicVar("SummonGainPercent", 10m)
    ];
    //定义可变参数:伤害基准数值，初始为10;每点奥斯提当前生命提供的额外伤害，初始为1;计算后的伤害数值;召唤数值，初始为1;击杀后增加的召唤数值占敌人最大生命的百分比，初始为10
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];
    //定义提示:提示消耗的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await OstyCmd.Summon(choiceContext, Owner, DynamicVars.Summon.BaseValue, this);
        int target_max_health = cardPlay.Target.MaxHp;
        if (!Osty.CheckMissingWithAnim(Owner))
        {
            bool shouldTriggerFatal = cardPlay.Target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());
            AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromOsty(Owner.Osty, this, cardPlay).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
                .Execute(choiceContext);
            if (shouldTriggerFatal && attackCommand.Results.SelectMany((List<DamageResult> r) => r).Any((DamageResult r) => r.WasTargetKilled))
            {
                int gain = (int)Math.Floor(target_max_health * DynamicVars["SummonGainPercent"].BaseValue / 100m);
                IncreaseSummon(gain);
                (DeckVersion as CallFromBeyond)?.IncreaseSummon(gain);
            }
        }
    }
    //卡牌效果:进行召唤,然后奥斯提造成一次伤害，若此次伤害杀死了非爪牙的敌人则永久提高召唤数值，
    private void IncreaseSummon(int gain)
    {
        IncreasedSummon += gain;
    }
    //累加召唤数值
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
    //升级效果:添加保留关键词
}
