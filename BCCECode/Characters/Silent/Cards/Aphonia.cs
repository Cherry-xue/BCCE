using BCCE.BCCECode.Characters.Silent.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode.Characters.Silent.Cards;

// 无声-在你的回合结束时，每有一张手牌，就获得1点敏捷,下个回合结束时,失去等量敏捷
public class Aphonia : SilentCard
{
    public Aphonia() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    //定义卡牌基本属性：1能量，能力，罕见稀有度，目标为自身,图鉴可见
    {
    }
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<DexterityPower>()
    ];
    //定义提示:提示敏捷的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsUpgraded)
        {
            await PowerCmd.Apply<UpgradedAphoniaPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<AphoniaPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }
    }
    //卡牌效果:
}
