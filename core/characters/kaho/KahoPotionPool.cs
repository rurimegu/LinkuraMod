using BaseLib.Abstracts;
using Godot;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Characters.Kaho;

/// <summary>
/// Potion pool for Linkura potions.
/// </summary>
public class KahoPotionPool : CustomPotionPoolModel {
  public override Color LabOutlineColor => HinoshitaKaho.Color;

  public override string BigEnergyIconPath => "big_energy.png".CharacterUiPath("kaho");
  public override string TextEnergyIconPath => "text_energy.png".CharacterUiPath("kaho");
}
