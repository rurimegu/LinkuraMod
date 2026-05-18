using RuriMegu.Core.Characters.Kaho;
using STS2RitsuLib.Interop.AutoRegistration;

namespace RuriMegu.Core.Potions.Kaho;

/// <summary>
/// Base class for all Kaho potions.
/// Inheriting from this automatically places potions in the Kaho potion pool.
/// </summary>
[RegisterPotion(typeof(KahoPotionPool), Inherit = true)]
public abstract class KahoPotion : LinkuraPotion {
  public override string CharacterId => HinoshitaKaho.CHARACTER_ID;
}
