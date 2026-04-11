using System.Collections.Generic;
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Cards.Kaho.Basic.Attack;
using RuriMegu.Core.Cards.Kaho.Basic.Skill;
using RuriMegu.Core.Relics;
using RuriMegu.Core.Relics.Kaho;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Characters.Kaho;

/// <summary>
/// Hinoshita Kaho
/// </summary>
public class HinoshitaKaho : PlaceholderCharacterModel {
  public const string CharacterId = "kaho";
  public const string CharacterName = "Hinoshita Kaho";

  public static readonly Color Color = new("f8b400");

  public override Color NameColor => Color;
  public override Color MapDrawingColor => Color;
  public override CharacterGender Gender => CharacterGender.Feminine;

  /// <summary>
  /// Point the game's energy counter loader at Kaho's custom scene.
  /// Path format matches what BaseLib's PlaceholderCharacterModel.GetCustomEnergyCounterAssetPath
  /// expects: a mod-relative path without the leading "res://" prefix.
  /// </summary>
  public override string CustomEnergyCounterPath => "energy_counter.tscn".CharacterScenePath(CharacterId);

  public override int StartingHp => 80;

  public override IEnumerable<CardModel> StartingDeck => [
    ModelDb.Card<KahoStrike>(),
    ModelDb.Card<KahoStrike>(),
    ModelDb.Card<KahoStrike>(),
    ModelDb.Card<KahoStrike>(),
    ModelDb.Card<KahoDefend>(),
    ModelDb.Card<KahoDefend>(),
    ModelDb.Card<KahoDefend>(),
    ModelDb.Card<KahoDefend>(),
    ModelDb.Card<LinkuraEnergy>(),
    ModelDb.Card<WideHeart>(),
  ];

  public override IReadOnlyList<RelicModel> StartingRelics => [
    ModelDb.Relic<LinkuraSystem>(),
  ];

  public override CardPoolModel CardPool => ModelDb.CardPool<HinoshitaKahoCardPool>();
  public override RelicPoolModel RelicPool => ModelDb.RelicPool<KahoRelicPool>();
  public override PotionPoolModel PotionPool => ModelDb.PotionPool<KahoPotionPool>();

  // Asset paths - placeholder until custom art is added
  public override string CustomIconTexturePath => "character_icon.png".CharacterUiPath(CharacterId);
  public override string CustomCharacterSelectIconPath => "char_select.png".CharacterUiPath(CharacterId);
  public override string CustomCharacterSelectLockedIconPath => "char_select_locked.png".CharacterUiPath(CharacterId);
  public override string CustomMapMarkerPath => "map_marker.png".CharacterUiPath(CharacterId);
  public override string CustomCharacterSelectBg => "select_bg.tscn".CharacterScenePath(CharacterId);
}
