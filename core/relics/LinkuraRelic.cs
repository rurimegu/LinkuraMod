using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Relics;

/// <summary>
/// Base class for all Linkura relics.
/// </summary>
public abstract class LinkuraRelic : CustomRelicModel {
  public abstract string CharacterId { get; }
  public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath(CharacterId);
  protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath(CharacterId);
  protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath(CharacterId);
}
