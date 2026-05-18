using RuriMegu.Core.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace RuriMegu.Core.Potions;

/// <summary>
/// Base class for all Linkura mod potions.
/// Provides image path resolution based on character ID.
/// </summary>
public abstract class LinkuraPotion : ModPotionTemplate {
  public abstract string CharacterId { get; }

  public override string CustomImagePath =>
    $"{GetType().Name.PascalToSnakeCase()}.png".PotionImagePath(CharacterId);

  public override string CustomOutlinePath =>
    $"{GetType().Name.PascalToSnakeCase()}_outline.png".PotionImagePath(CharacterId);
}
