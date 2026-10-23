using BCCE.BCCECode.Characters.Defect.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BCCE.BCCECode.Characters.Defect.Cards;

// 开源-观看抽牌堆时，卡牌将按实际发牌顺序排列。
public class OpenSource : DefectCard
{
    public OpenSource() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    //定义卡牌基本属性：1能量，能力，罕见，目标为自身,图鉴可见
    {
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<OpenSourcePower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }
    //卡牌效果:获得开源能力
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
    //升级效果:添加固有关键词
}
