using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using RuriMegu.Core.Characters;

namespace RuriMegu.Core.Utils;

public static class LinkuraAnimation {
  public const string TRIGGER_NAME_CAST = "Cast";
  public const string LINKURA_ANIM_BURST = "burst";
  public const string LINKURA_ANIM_COLLECT = "collect";

  public const string VANILLA_ANIM_IDLE = "idle_loop";
  public const string VANILLA_ANIM_DIE = "die";
  public const string VANILLA_ANIM_ATTACK = "attack";
  public const string VANILLA_ANIM_CAST = "cast";
  public const string VANILLA_ANIM_HURT = "hurt";
  public const string VANILLA_ANIM_RELAXED_LOOP = "relaxed_loop";
  public const string VANILLA_ANIM_REST_SITE_ACT1 = "overgrowth_loop";
  public const string VANILLA_ANIM_REST_SITE_ACT2 = "hive_loop";
  public const string VANILLA_ANIM_REST_SITE_ACT3 = "glory_loop";

  public static readonly ImmutableDictionary<string, string> MAPPED_ANIMATIONS
    = ImmutableDictionary.CreateRange([
      KeyValuePair.Create(VANILLA_ANIM_IDLE, "quest_dance_general00"),
      KeyValuePair.Create(VANILLA_ANIM_DIE, "quest_dance_mentaldown"),
      KeyValuePair.Create(VANILLA_ANIM_ATTACK, "quest_dance_general03"),
      KeyValuePair.Create(VANILLA_ANIM_CAST, "quest_dance_general14"),
      KeyValuePair.Create(VANILLA_ANIM_HURT, "quest_dance_surprise02_in"),
      KeyValuePair.Create(VANILLA_ANIM_RELAXED_LOOP, "quest_skill_moodmaker05_loop"),
      KeyValuePair.Create(LINKURA_ANIM_BURST, "quest_skill_performer01"),
      KeyValuePair.Create(LINKURA_ANIM_COLLECT, "quest_dance_general34"),
      KeyValuePair.Create(VANILLA_ANIM_REST_SITE_ACT1, "quest_dance_general01"),
      KeyValuePair.Create(VANILLA_ANIM_REST_SITE_ACT2, "quest_dance_general01"),
      KeyValuePair.Create(VANILLA_ANIM_REST_SITE_ACT3, "quest_dance_general01"),
    ]);

  public static async Task PlayCastAnim(this Player player) {
    if (player.Character is ILinkuraCharacter linkuraChara) {
      await CreatureCmd.TriggerAnim(player.Creature, TRIGGER_NAME_CAST, linkuraChara.CastAnimDelay);
    }
  }

  public static async Task PlayBurstAnim(this Player player) {
    if (player.Character is ILinkuraCharacter linkuraChara) {
      await PlayCustomSpineAnim(player, linkuraChara.GetMappedAnimation(LINKURA_ANIM_BURST),
        linkuraChara.GetMappedAnimation(VANILLA_ANIM_IDLE), linkuraChara.BurstAnimDelay);
    }
  }

  public static async Task PlayCollectAnim(this Player player) {
    if (player.Character is ILinkuraCharacter linkuraChara) {
      await PlayCustomSpineAnim(player, linkuraChara.GetMappedAnimation(LINKURA_ANIM_COLLECT),
        linkuraChara.GetMappedAnimation(VANILLA_ANIM_IDLE), linkuraChara.CollectAnimDelay);
    }
  }

  // CreatureCmd.TriggerAnim goes through CreatureAnimator's state machine, which only fires
  // registered trigger branches. Custom Spine animations (burst/collect) are not in the state
  // machine, so we bypass it and drive the SpineAnimationState directly.
  private static async Task PlayCustomSpineAnim(Player player, string animName, string idleAnimName, float waitTime) {
    var creature = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
    if (creature == null || creature.Visuals.SpineBody?.HasAnimation(animName) != true) {
      LinkuraMod.Logger.Warn($"Could not play animation '{animName}' - SpineController or animation not found");
      return;
    }

    var spineAnim = creature.SpineAnimation;
    spineAnim.SetAnimation(animName, false);
    spineAnim.AddAnimation(idleAnimName, 0f, true);

    await Cmd.CustomScaledWait(Mathf.Min(waitTime * 0.5f, 0.25f), waitTime);
  }
}
