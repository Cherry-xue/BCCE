using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace BCCE.BCCECode;

[RegisterOwnedCardKeyword(nameof(EliteImbued), CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
public class BCCEKeywords
{
    public static readonly CardKeyword EliteImbued = ModContentRegistry.GetQualifiedKeywordId(MainFile.ModId, nameof(EliteImbued)).GetModCardKeyword();
}