using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BCCE.BCCECode.Characters.Ironclad.Powers;

[RegisterPower(Inherit = true)]
public abstract class SilentPower : ModPowerTemplate
{
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://BCCE/Images/Powers/{GetType().Name}.png",
        BigIconPath: $"res://BCCE/Images/Powers/{GetType().Name}.png"
    );
}
