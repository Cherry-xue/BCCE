using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace BCCE.BCCECode.Characters.Necrobinder.Powers;

// 指挥-在自己的回合开始时，每名其他玩家提供等同于能力层数的额外召唤。
public sealed class CommandPower : NecrobinderPower
{
    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益

    public override PowerStackType StackType => PowerStackType.Counter;
    //定义叠加类型：计数器，层数表示每名其他玩家提供的召唤值

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.SummonStatic)
    ];
    //定义提示：显示召唤的相关信息

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || AmountOnTurnStart == 0)
        {
            return;
        }
        int otherPlayers = Math.Max(0, CombatState.Players.Count - 1);
        if (otherPlayers > 0)
        {
            Flash();
            await OstyCmd.Summon(choiceContext, player, Amount * otherPlayers, this);
        }
    }
    //在自己的回合开始后触发：沿用原版下回合召唤的时机，按队伍总人数减去自己计算额外召唤，不计奥斯提等宠物
}
