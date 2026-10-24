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

// 聚力打击的临时力量-记录本次获得的力量，在下个己方回合结束前撤回。
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
    //定义叠加类型：计数器，层数表示本次临时力量的实际数量
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    //定义实例类型：独立实例，不同次授予分别记录数值与到期时间
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
    //初始化内部数据：为每个临时力量实例单独记录授予回合

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
        if (participants.Contains(Owner) && Owner.Player.PlayerCombatState.TurnNumber > GetInternalData<Data>().grantedTurn)
        {
            await PowerCmd.Remove(this);
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -Amount, Owner, null);
        }
    }
    //在下个己方回合结束的较早阶段触发：仅撤回本实例记录的力量，授予当回合与敌方回合均不撤回
}
