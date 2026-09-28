using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Interop;

namespace BCCE.BCCECode;

[ModInitializer(nameof(Initialize))]
public class MainFile
{
    public const string ModId = "BCCE";
    public static readonly Logger Logger = RitsuLibFramework.CreateLogger(ModId);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();
        var assembly = Assembly.GetExecutingAssembly();
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        // 自动注册内容
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
    }
}