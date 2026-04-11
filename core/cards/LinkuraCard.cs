using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using RuriMegu.Core.Characters.Kaho;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards;

/// <summary>
/// Base class for all Linkura-pool cards.
/// Inheriting from this automatically places cards in the Linkura card pool.
/// </summary>
public abstract class LinkuraCard(int cost, CardType type, CardRarity rarity, TargetType target)
  : CustomCardModel(cost, type, rarity, target) {
  public virtual string CharacterId => "";
  public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath(CharacterId);
  public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath(CharacterId);
  public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath(CharacterId);
}
