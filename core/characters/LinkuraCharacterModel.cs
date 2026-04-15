using System.Collections.Generic;
using System.Collections.Immutable;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Characters;

/// <summary>
/// Hinoshita Kaho
/// </summary>
public abstract class LinkuraCharacterModel : PlaceholderCharacterModel {
  public abstract string CharacterId { get; }
  public abstract string CharacterName { get; }
  public virtual float BurstAnimDelay => 0.1f;
  public virtual float CollectAnimDelay => 0.1f;

  public override CharacterGender Gender => CharacterGender.Feminine;

  /// <summary>
  /// Point the game's energy counter loader at Kaho's custom scene.
  /// Path format matches what BaseLib's PlaceholderCharacterModel.GetCustomEnergyCounterAssetPath
  /// expects: a mod-relative path without the leading "res://" prefix.
  /// </summary>
  public override string CustomVisualPath => "character_visuals.tscn".CharacterScenePath(CharacterId);
  public override string CustomMerchantAnimPath => "character_merchant.tscn".CharacterScenePath(CharacterId);
  public override string CustomRestSiteAnimPath => "character_rest_site.tscn".CharacterScenePath(CharacterId);
  public override string CustomEnergyCounterPath => "energy_counter.tscn".CharacterScenePath(CharacterId);

  // Asset paths - placeholder until custom art is added
  public override string CustomIconTexturePath => "character_icon.png".CharacterUiPath(CharacterId);
  public override string CustomCharacterSelectIconPath => "char_select.png".CharacterUiPath(CharacterId);
  public override string CustomCharacterSelectLockedIconPath => "char_select_locked.png".CharacterUiPath(CharacterId);
  public override string CustomMapMarkerPath => "map_marker.png".CharacterUiPath(CharacterId);
  public override string CustomCharacterSelectBg => "select_bg.tscn".CharacterScenePath(CharacterId);

  // Multiplayer mode - arm pointer.
  public override string CustomArmPointingTexturePath => "hand_pointer.png".CharacterUiPath(CharacterId);
  // Multiplayer mode - rock-paper-scissors.
  public override string CustomArmRockTexturePath => "hand_rock.png".CharacterUiPath(CharacterId);
  public override string CustomArmPaperTexturePath => "hand_paper.png".CharacterUiPath(CharacterId);
  public override string CustomArmScissorsTexturePath => "hand_scissors.png".CharacterUiPath(CharacterId);

  public static readonly ImmutableDictionary<string, string> MAPPED_ANIMATIONS
    = ImmutableDictionary.CreateRange([
      KeyValuePair.Create(LinkuraAnimation.VANILLA_ANIM_IDLE, "quest_dance_general00"),
      KeyValuePair.Create(LinkuraAnimation.VANILLA_ANIM_DIE, "quest_dance_mentaldown"),
      KeyValuePair.Create(LinkuraAnimation.VANILLA_ANIM_ATTACK, "quest_dance_general03"),
      KeyValuePair.Create(LinkuraAnimation.VANILLA_ANIM_CAST, "quest_dance_general14"),
      KeyValuePair.Create(LinkuraAnimation.VANILLA_ANIM_HURT, "quest_dance_surprise02_in"),
      KeyValuePair.Create(LinkuraAnimation.VANILLA_ANIM_RELAXED_LOOP, "quest_skill_moodmaker05_loop"),
      KeyValuePair.Create(LinkuraAnimation.LINKURA_ANIM_BURST, "quest_skill_performer01"),
      KeyValuePair.Create(LinkuraAnimation.LINKURA_ANIM_COLLECT, "quest_dance_general34"),
      KeyValuePair.Create(LinkuraAnimation.VANILLA_ANIM_REST_SITE_ACT1, "quest_dance_general01"),
      KeyValuePair.Create(LinkuraAnimation.VANILLA_ANIM_REST_SITE_ACT2, "quest_dance_general01"),
      KeyValuePair.Create(LinkuraAnimation.VANILLA_ANIM_REST_SITE_ACT3, "quest_dance_general01"),
    ]);

  /// <summary>
  /// Maps a vanilla animation name to the actual animation name in the Spine file.
  /// Derived classes can override this to provide specific mappings.
  /// </summary>
  public virtual string GetMappedAnimation(string vanillaName) {
    if (MAPPED_ANIMATIONS.TryGetValue(vanillaName, out string anim))
      return anim;
    LinkuraMod.Logger.Error($"Unknown vanilla animation name: {vanillaName}");
    return MAPPED_ANIMATIONS.GetValueOrDefault(LinkuraAnimation.VANILLA_ANIM_IDLE);
  }

  public override CreatureAnimator GenerateAnimator(MegaSprite controller) {
    string idleAnim = GetMappedAnimation(LinkuraAnimation.VANILLA_ANIM_IDLE);
    string castAnim = GetMappedAnimation(LinkuraAnimation.VANILLA_ANIM_CAST);
    string attackAnim = GetMappedAnimation(LinkuraAnimation.VANILLA_ANIM_ATTACK);
    string hurtAnim = GetMappedAnimation(LinkuraAnimation.VANILLA_ANIM_HURT);
    string dieAnim = GetMappedAnimation(LinkuraAnimation.VANILLA_ANIM_DIE);
    string relaxedLoopAnim = GetMappedAnimation(LinkuraAnimation.VANILLA_ANIM_RELAXED_LOOP);

    AnimState animState = new(idleAnim, isLooping: true);
    AnimState animState2 = new(castAnim);
    AnimState animState3 = new(attackAnim);
    AnimState animState4 = new(hurtAnim);
    AnimState state = new(dieAnim);
    AnimState animState5 = new(relaxedLoopAnim, isLooping: true);

    animState2.NextState = animState;
    animState3.NextState = animState;
    animState4.NextState = animState;
    animState5.AddBranch("Idle", animState);

    CreatureAnimator creatureAnimator = new(animState, controller);
    creatureAnimator.AddAnyState("Idle", animState);
    creatureAnimator.AddAnyState("Dead", state);
    creatureAnimator.AddAnyState("Hit", animState4);
    creatureAnimator.AddAnyState("Attack", animState3);
    creatureAnimator.AddAnyState("Cast", animState2);
    creatureAnimator.AddAnyState("Relaxed", animState5);

    return creatureAnimator;
  }
}
