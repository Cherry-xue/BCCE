using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;

namespace BCCE.BCCECode.Characters.Necrobinder.Powers;

// 奈何行-本回合内，你每打出3张灵魂，获得1点能量
public sealed class HelplessWalkPower : NecrobinderPower
{
    private class Data
    {
        public int soulPlayed;
        public int triggerCount;
    }
    //定义内部数据类，包含本回合已打出的灵魂数量与已触发次数
    private const int _soulIncrement = 3;
    //定义灵魂触发增量为3
    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益
    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    //定义实例类型：独立实例
    public override int DisplayAmount => _soulIncrement - GetInternalData<Data>().soulPlayed % _soulIncrement;
    //定义显示数值：距离下一次触发还需要打出的灵魂数量
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromCard<Soul>()
    ];
    //定义额外的悬停提示：显示灵魂卡牌的悬停提示
    protected override object InitInternalData()
    {
        return new Data();
    }
    //初始化内部数据：返回一个新的Data对象
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card is not Soul || cardPlay.Card.Owner.Creature != Owner)
        {
            return;
        }
        Data data = GetInternalData<Data>();
        data.soulPlayed++;
        int triggers = data.soulPlayed / _soulIncrement - data.triggerCount;
        if (triggers > 0)
        {
            Flash();
            await PlayerCmd.GainEnergy(Amount * triggers, Owner.Player);
            data.triggerCount += triggers;
        }
        InvokeDisplayAmountChanged();
    }
    //在卡牌打出后触发：累计本回合打出的灵魂数量，每满3张就获得能量，最后更新显示数值
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Remove<HelplessWalkPower>(Owner);
        }
    }
    //在回合结束前触发：本能力只在本回合生效，回合结束时移除自身
}
