using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;

namespace RuriMegu.Core.Cards;

[RegisterOwnedCardKeyword(nameof(Collect))]
[RegisterOwnedCardKeyword(nameof(Backstage))]
public sealed class LinkuraKeywords {
  public static readonly string Collect = ModContentRegistry.GetQualifiedKeywordId(LinkuraMod.ModId, nameof(Collect));
  public static readonly string Backstage = ModContentRegistry.GetQualifiedKeywordId(LinkuraMod.ModId, nameof(Backstage));
}
