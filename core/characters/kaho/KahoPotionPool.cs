using Godot;
using RuriMegu.Core.Utils;
using STS2RitsuLib.Scaffolding.Content;

namespace RuriMegu.Core.Characters.Kaho;

/// <summary>
/// Potion pool for Linkura potions.
/// </summary>
public class KahoPotionPool : TypeListPotionPoolModel {
  public override string EnergyColorName => HinoshitaKaho.CHARACTER_ID;
  public override Color LabOutlineColor => HinoshitaKaho.Color;

  public override string BigEnergyIconPath => "big_energy.png".CharacterUiPath(HinoshitaKaho.CHARACTER_ID);
  public override string TextEnergyIconPath => "text_energy.png".CharacterUiPath(HinoshitaKaho.CHARACTER_ID);
}
