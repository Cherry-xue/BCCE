using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rooms;

namespace BCCE.BCCECode.Patch;

// 猩红渴求-记录本场战斗中需要屏蔽燃烧之血/黑暗之血治疗的玩家
public static class BloodHealSuppressor
{
    private static readonly Dictionary<Player, CombatRoom> _markedRooms = new();
    //定义静态字典，记录每个玩家被标记的战斗房间
    public static void Mark(Player player, CombatRoom room)
    {
        _markedRooms[player] = room;
    }
    //标记该玩家在本场战斗中需要屏蔽血族遗物的治疗
    public static bool IsMarked(Player player, CombatRoom room)
    {
        return _markedRooms.TryGetValue(player, out CombatRoom markedRoom) && markedRoom == room;
    }
    //判断该玩家是否在本场战斗中被标记：以房间比对，避免沿用上一场战斗的记录
}
