using Godot;
using RuriMegu.Core.Utils;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace RuriMegu.Core.Characters.Kaho;

/// <summary>
/// Card pool for Hinoshita Kaho-colored cards.
/// </summary>
public class HinoshitaKahoCardPool : TypeListCardPoolModel {
  public override string EnergyColorName => HinoshitaKaho.CHARACTER_ID;

  public override string Title => HinoshitaKaho.CHARACTER_NAME;

  public override string BigEnergyIconPath => "big_energy.png".CharacterUiPath(HinoshitaKaho.CHARACTER_ID);
  public override string TextEnergyIconPath => "text_energy.png".CharacterUiPath(HinoshitaKaho.CHARACTER_ID);

  public override Material PoolFrameMaterial =>
    MaterialUtils.CreateHsvShaderMaterial(0.121f, 1.0f, 0.9725f);

  public override Color DeckEntryCardColor => HinoshitaKaho.Color;

  public override bool IsColorless => false;
}
