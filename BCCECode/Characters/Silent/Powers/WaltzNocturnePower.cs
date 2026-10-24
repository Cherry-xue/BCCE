using BCCE.BCCECode.Characters.Silent.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode.Characters.Silent.Powers;

// 华尔兹夜曲-记录本回合额外获得的鬼祟，在回合结束后扣回对应层数。
public sealed class WaltzNocturnePower : SilentPower, ITemporaryPower
{
    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益

    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器，同回合重复打出时累加临时鬼祟层数

    public AbstractModel OriginModel => ModelDb.Card<WaltzNocturne>();
    //定义临时能力来源：华尔兹夜曲

    public PowerModel InternallyAppliedPower => ModelDb.Power<SneakyPower>();
    //声明本临时能力关联的原版鬼祟，供原版识别两者关系；实际施加由下方事件完成
    //能力图标沿用SilentPower的独立路径：res://BCCE/Images/Powers/WaltzNocturnePower.png

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<SneakyPower>()
    ];
    //定义提示：显示原版鬼祟能力的相关信息

    public override async Task BeforeApplied(Creature target, decimal amount, Creature applier, CardModel cardSource)
    {
        await PowerCmd.Apply<SneakyPower>(new ThrowingPlayerChoiceContext(), target, amount, applier, cardSource, silent: true);
    }
    //在首次施加前触发：参照原版临时力量，使目标获得等量的原版鬼祟

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature applier, CardModel cardSource)
    {
        if (power == this && amount != Amount)
        {
            await PowerCmd.Apply<SneakyPower>(choiceContext, Owner, amount, applier, cardSource, silent: true);
        }
    }
    //在层数改变后触发：首次施加已由BeforeApplied处理，后续叠加或减少时同步调整鬼祟层数

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
            var sneaky = Owner.GetPower<SneakyPower>();
            if (sneaky != null)
            {
                await PowerCmd.ModifyAmount(choiceContext, sneaky, -Math.Min(Amount, sneaky.Amount), Owner, null);
            }
        }
    }
    //在所属方回合结束后触发：扣回本能力提供的临时鬼祟，保留其他来源的永久层数并移除本能力
}
