using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Characters;

/// <summary>
/// Hinoshita Kaho
/// </summary>
public abstract class LinkuraCharacterModel : PlaceholderCharacterModel {
  public abstract string CharacterId { get; }
  public abstract string CharacterName { get; }
  public virtual string AnimNameBurst => LinkuraAnimation.ANIM_NAME_BURST;
  public virtual float BurstAnimDelay => 0.1f;
  public virtual string AnimNameCollect => LinkuraAnimation.ANIM_NAME_COLLECT;
  public virtual float CollectAnimDelay => 0.1f;

  public override CharacterGender Gender => CharacterGender.Feminine;

  /// <summary>
  /// Point the game's energy counter loader at Kaho's custom scene.
  /// Path format matches what BaseLib's PlaceholderCharacterModel.GetCustomEnergyCounterAssetPath
  /// expects: a mod-relative path without the leading "res://" prefix.
  /// </summary>
  public override string CustomVisualPath => "character_visuals.tscn".CharacterScenePath(CharacterId);
  public override string CustomEnergyCounterPath => "energy_counter.tscn".CharacterScenePath(CharacterId);

  // Asset paths - placeholder until custom art is added
  public override string CustomIconTexturePath => "character_icon.png".CharacterUiPath(CharacterId);
  public override string CustomCharacterSelectIconPath => "char_select.png".CharacterUiPath(CharacterId);
  public override string CustomCharacterSelectLockedIconPath => "char_select_locked.png".CharacterUiPath(CharacterId);
  public override string CustomMapMarkerPath => "map_marker.png".CharacterUiPath(CharacterId);
  public override string CustomCharacterSelectBg => "select_bg.tscn".CharacterScenePath(CharacterId);
}
