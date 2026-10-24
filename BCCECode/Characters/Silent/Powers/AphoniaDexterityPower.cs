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
using STS2RitsuLib.Scaffolding.Content;

namespace BCCE.BCCECode.Characters.Silent.Powers;

// 无声的临时敏捷-记录本次获得的敏捷，在下个己方回合结束前撤回。
public sealed class AphoniaDexterityPower : SilentPower, ITemporaryPower
{
    private class Data
    {
        public int grantedTurn;
    }
    //定义内部数据：记录授予敏捷时玩家自己的回合编号

    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益
    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器，层数表示本次临时敏捷的实际数量
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    //定义实例类型：独立实例，不同次授予分别记录数值与到期时间
    public AbstractModel OriginModel => ModelDb.Card<Aphonia>();
    //定义临时能力来源：无声
    public PowerModel InternallyAppliedPower => ModelDb.Power<DexterityPower>();
    //声明关联原版敏捷，供原版识别临时能力与内部敏捷变化的关系

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://BCCE/Images/Powers/AphoniaPower.png",
        BigIconPath: "res://BCCE/Images/Powers/AphoniaPower.png"
    );
    //定义能力图标：复用无声的能力图

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<DexterityPower>()
    ];
    //定义提示：显示敏捷的相关信息

    protected override object InitInternalData()
    {
        return new Data();
    }
    //初始化内部数据：为每个临时敏捷实例单独记录授予回合

    public override async Task BeforeApplied(Creature target, decimal amount, Creature applier, CardModel cardSource)
    {
        GetInternalData<Data>().grantedTurn = target.Player.PlayerCombatState.TurnNumber;
        await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), target, amount, applier, cardSource, silent: true);
    }
    //在首次施加前触发：记录玩家回合编号并给予等量敏捷，数值不再随手牌变化而重新计算

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature applier, CardModel cardSource)
    {
        if (power == this && amount != Amount)
        {
            await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, amount, applier, cardSource, silent: true);
        }
    }
    //在层数变化后触发：首次施加已单独处理，后续层数变化同步调整敏捷

    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner) && Owner.Player.PlayerCombatState.TurnNumber > GetInternalData<Data>().grantedTurn)
        {
            await PowerCmd.Remove(this);
            await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, -Amount, Owner, null);
        }
    }
    //在下个己方回合结束的较早阶段触发：先撤回旧敏捷，再由无声结算新敏捷，授予当回合与敌方回合均不撤回
}
