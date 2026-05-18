using Godot;
using RuriMegu.Core.Characters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace RuriMegu.Core.Characters.Kaho;

/// <summary>
/// Hinoshita Kaho
/// </summary>
[RegisterCharacter]
public class HinoshitaKaho : LinkuraCharacterModel<HinoshitaKahoCardPool, KahoRelicPool, KahoPotionPool> {
  public const string CHARACTER_ID = "kaho";
  public const string CHARACTER_NAME = "Hinoshita Kaho";
  public override string CharacterId => CHARACTER_ID;
  public override string CharacterName => CHARACTER_NAME;

  public static readonly Color Color = new("f8b400");

  public override Color NameColor => Color;
  public override Color MapDrawingColor => Color;
  public override Color EnergyLabelOutlineColor => Color;

  public override int StartingHp => 80;
}
