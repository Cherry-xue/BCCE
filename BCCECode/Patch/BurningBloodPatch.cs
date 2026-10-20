using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace BCCE.BCCECode.Patch;

// 猩红渴求-使燃烧之血在本场战斗结束时不再提供生命恢复
[HarmonyPatch(typeof(BurningBlood), nameof(BurningBlood.AfterCombatVictory))]
public static class BurningBloodPatch
{
    private static bool Prefix(BurningBlood __instance, CombatRoom _, ref Task __result)
    {
        if (!BloodHealSuppressor.IsMarked(__instance.Owner, _))
        {
            return true;
        }
        __result = Task.CompletedTask;
        return false;
    }
    //前缀补丁：若本场战斗被猩红渴求标记，则跳过燃烧之血原本的战斗胜利治疗
    //注意：Prefix 里的参数名必须与原方法的参数名一致（原版写的是 CombatRoom _），
    //Harmony 按参数名匹配，写成别的名字会导致 PatchAll 抛异常、整个 mod 初始化中断
}
