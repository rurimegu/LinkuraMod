using BaseLib.Abstracts;
using Godot;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Characters.Kaho;

/// <summary>
/// Relic pool for Linkura relics.
/// </summary>
public class KahoRelicPool : CustomRelicPoolModel {
  public override Color LabOutlineColor => HinoshitaKaho.Color;

  public override string BigEnergyIconPath => "big_energy.png".CharacterUiPath("kaho");
  public override string TextEnergyIconPath => "text_energy.png".CharacterUiPath("kaho");
}
