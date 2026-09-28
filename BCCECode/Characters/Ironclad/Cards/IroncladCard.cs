using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BCCE.BCCECode.Characters.Ironclad.Cards;

[RegisterCard(typeof(IroncladCardPool), Inherit = true)]
public abstract class IroncladCard(int cost, CardType type, CardRarity rarity, TargetType target, bool shouldShowInCardLibrary) :
    ModCardTemplate(cost, type, rarity, target, shouldShowInCardLibrary)
{
    public override CardAssetProfile AssetProfile => new(
    PortraitPath: $"res://BCCE/Images/Cards/{GetType().Name}.png"
);
}
