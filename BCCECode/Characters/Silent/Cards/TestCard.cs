using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BCCE.BCCECode.Characters.Silent.Cards;

// 打击-对目标造成伤害
public class TestCard : SilentCard
{
    public TestCard() : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy, true)
    //定义卡牌基本属性：0能量，攻击，基础稀有度，目标为任意敌人
    {
    }
    public override bool CanBeGeneratedInCombat => false;
    //定义不能在战斗中生成
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Log.Info(">>>[NonoMod]怪物最大生命=" + cardPlay.Target.Monster.Creature.MaxHp);
        Log.Info(">>>[NonoMod]怪物当前生命=" + cardPlay.Target.Monster.Creature.CurrentHp);
        Creature target = cardPlay.Target;

        if (target.Monster is MonsterModel monster)
        {
            int intentDamage = monster.NextMove.Intents
                .OfType<AttackIntent>()
                .Sum(intent => intent.GetTotalDamage(new[] { target }, monster.Creature));

            // intentDamage 就是该敌人当前攻击意图预计造成的总伤害
            Log.Info(">>>[NonoMod]怪物当前意图总伤害=" + intentDamage);
        }
        
    }
}
