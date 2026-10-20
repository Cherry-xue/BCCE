using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BCCE.BCCECode.Characters.Ironclad.Powers;

// 弱者-记录失去的力量层数与剩余的持续回合，持续回合归零时返还力量
[RegisterPower(Inherit = true)]
public class WeaklingPower : IroncladPower
{
    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://BCCE/Images/Powers/WeaklingPower.png",
        BigIconPath: "res://BCCE/Images/Powers/WeaklingPower.png"
    );
    //定义能力图标：三个同名的弱者能力共用同一张图标
    public override PowerType Type => PowerType.Debuff;
    //定义能力类型：减益
    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器
    public override int DisplayAmount => Amount;
    //定义显示数值:失去的力量层数
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Duration", 1m)
    ];
    //定义可变参数:剩余的持续回合数，初始为1
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    //定义额外的悬停提示：显示力量的悬停提示
    public void SetDuration(int duration)
    {
        DynamicVars["Duration"].BaseValue = duration;
    }
    //设置剩余的持续回合数，由弱肉强食在施加本能力时调用
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            DynamicVars["Duration"].BaseValue -= 1m;
            if (DynamicVars["Duration"].BaseValue <= 0m)
            {
                await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, Amount, Owner, null);
                await PowerCmd.Remove(this);
            }
        }
    }
    //在回合结束前触发：剩余持续回合数减少1，归零时返还失去的力量并移除本能力
}
