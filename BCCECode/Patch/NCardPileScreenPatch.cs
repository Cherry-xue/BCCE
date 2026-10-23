using System.Collections.Generic;
using System.Reflection;
using BCCE.BCCECode.Characters.Defect.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

namespace BCCE.BCCECode.Patch;

// 开源-拥有该能力时，抽牌堆界面按实际发牌顺序显示，而不是按稀有度排序
[HarmonyPatch(typeof(NCardPileScreen), "OnPileContentsChanged")]
public static class NCardPileScreenPatch
{
    private static readonly FieldInfo GridField =
        typeof(NCardPileScreen).GetField("_grid", BindingFlags.NonPublic | BindingFlags.Instance);
    //定义反射字段：界面里的卡牌网格是私有字段，只能靠反射取到

    private static void Postfix(NCardPileScreen __instance)
    {
        CardPile pile = __instance.Pile;
        if (pile == null || pile.Type != PileType.Draw)
        {
            return;
        }
        List<CardModel> cards = pile.Cards.ToList();
        if (cards.Count == 0)
        {
            return;
        }
        if (cards[0].Owner == null || !cards[0].Owner.Creature.HasPower<OpenSourcePower>())
        {
            return;
        }
        NCardGrid grid = (NCardGrid)GridField.GetValue(__instance);
        grid?.SetCards(cards, pile.Type, new List<SortingOrders> { SortingOrders.Ascending });
    }
    //后缀补丁：原版会先按稀有度排序再显示，这里对拥有开源能力的玩家改用未排序的牌序重新填充网格，覆盖掉排序结果
    //注意：OnPileContentsChanged 是私有方法，只能按方法名字符串挂载；若官方改名会导致 PatchAll 失败
}
