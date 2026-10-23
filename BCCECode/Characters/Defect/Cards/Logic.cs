using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace BCCE.BCCECode.Characters.Defect.Cards;

// 逻辑-如果有空余充能球栏位，生成2个冰霜充能球；否则消耗这张牌并获得1个充能球栏位。
public class Logic : DefectCard
{
    public Logic() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    //定义卡牌基本属性：2能量，技能，罕见，目标为自身,图鉴可见
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new RepeatVar(2)
    ];
    //定义可变参数:生成的冰霜充能球数量，初始为2
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromOrb<FrostOrb>()
    ];
    //定义提示:提示冰霜充能球的相关信息
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        OrbQueue orbQueue = Owner.PlayerCombatState.OrbQueue;
        if (orbQueue.Capacity > orbQueue.Orbs.Count)
        {
            for (int i = 0; i < DynamicVars.Repeat.IntValue; i++)
            {
                await OrbCmd.Channel<FrostOrb>(choiceContext, Owner);
            }
        }
        else
        {
            await CardCmd.Exhaust(choiceContext, this);
            await OrbCmd.AddSlots(Owner, 1);
        }
    }
    //卡牌效果:空余栏位只判断一次，只要有空位就连续生成2个冰霜充能球；
    //原版 OrbCmd.Channel 在栏位已满时会先激发最左侧的充能球腾出位置，因此不会生成失败
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    //升级效果:能量消耗减少1
}
