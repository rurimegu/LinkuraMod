using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using RuriMegu.Core.Characters;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Relics;

/// <summary>
/// Base class for all Linkura-pool relics.
/// Inheriting from this automatically places relics in the Linkura relic pool.
/// </summary>
[Pool(typeof(LinkuraRelicPool))]
public abstract class LinkuraRelic : CustomRelicModel {
  public override RelicRarity Rarity => RelicRarity.Common;

  public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
  protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
  protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}
