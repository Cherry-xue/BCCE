using BCCE.BCCECode.Characters.Ironclad.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace BCCE.BCCECode.Characters.Ironclad.Powers;

// 聚力打击的临时力量-汇总累计力量；到期时若拥有聚力打击则保留，否则一次性撤回。
public sealed class ChargedStrikeStrengthPower : IroncladPower, ITemporaryPower
{
    private class Data
    {
        public int grantedTurn;
    }
    //定义内部数据：记录授予力量时玩家自己的回合编号

    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益
    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器，层数表示累计获得的聚力临时力量
    public AbstractModel OriginModel => ModelDb.Card<ChargedStrike>();
    //定义临时能力来源：聚力打击
    public PowerModel InternallyAppliedPower => ModelDb.Power<StrengthPower>();
    //声明关联原版力量，供原版识别临时能力与内部力量变化的关系

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://BCCE/Images/Powers/ChargedStrikePower.png",
        BigIconPath: "res://BCCE/Images/Powers/ChargedStrikePower.png"
    );
    //定义能力图标：复用聚力打击的能力图

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    //定义提示：显示力量的相关信息

    protected override object InitInternalData()
    {
        return new Data();
    }
    //初始化内部数据：记录首次授予回合，避免刚获得的力量在当回合提前消失

    public override async Task BeforeApplied(Creature target, decimal amount, Creature applier, CardModel cardSource)
    {
        GetInternalData<Data>().grantedTurn = target.Player.PlayerCombatState.TurnNumber;
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), target, amount, applier, cardSource, silent: true);
    }
    //在首次施加前触发：记录玩家回合编号并给予等量力量，数值不再随之后出牌而重新计算

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature applier, CardModel cardSource)
    {
        if (power == this && amount != Amount)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, amount, applier, cardSource, silent: true);
        }
    }
    //在层数变化后触发：首次施加已单独处理，后续层数变化同步调整力量

    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)
            && Owner.Player.PlayerCombatState.TurnNumber > GetInternalData<Data>().grantedTurn
            && !Owner.HasPower<ChargedStrikePower>())
        {
            int strengthToRemove = Amount;
            await PowerCmd.Remove(this);
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -strengthToRemove, Owner, null);
        }
    }
    //在己方回合结束的较早阶段触发：首次授予当回合保留；之后若有待结算聚力则续接，否则撤回全部累计力量
}
