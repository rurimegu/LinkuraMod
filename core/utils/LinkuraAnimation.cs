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

  public static async Task PlayCastAnim(this Player player) {
    if (player.Character is LinkuraCharacterModel linkuraChara) {
      await CreatureCmd.TriggerAnim(player.Creature, TRIGGER_NAME_CAST, linkuraChara.CastAnimDelay);
    }
  }

  public static async Task PlayBurstAnim(this Player player) {
    if (player.Character is LinkuraCharacterModel linkuraChara) {
      await PlayCustomSpineAnim(player, linkuraChara.GetMappedAnimation(LINKURA_ANIM_BURST),
        linkuraChara.GetMappedAnimation(VANILLA_ANIM_IDLE), linkuraChara.BurstAnimDelay);
    }
  }

  public static async Task PlayCollectAnim(this Player player) {
    if (player.Character is LinkuraCharacterModel linkuraChara) {
      await PlayCustomSpineAnim(player, linkuraChara.GetMappedAnimation(LINKURA_ANIM_COLLECT),
        linkuraChara.GetMappedAnimation(VANILLA_ANIM_IDLE), linkuraChara.CollectAnimDelay);
    }
  }

  // CreatureCmd.TriggerAnim goes through CreatureAnimator's state machine, which only fires
  // registered trigger branches. Custom Spine animations (burst/collect) are not in the state
  // machine, so we bypass it and drive the SpineAnimationState directly.
  private static async Task PlayCustomSpineAnim(Player player, string animName, string idleAnimName, float waitTime) {
    var spine = NCombatRoom.Instance?.GetCreatureNode(player.Creature)?.SpineController;
    if (spine == null || !spine.HasAnimation(animName)) {
      LinkuraMod.Logger.Warn($"Could not play animation '{animName}' - SpineController or animation not found");
      return;
    }

    var animState = spine.GetAnimationState();
    animState.SetAnimation(animName, false);
    animState.AddAnimation(idleAnimName, 0f, true);

    await Cmd.CustomScaledWait(Mathf.Min(waitTime * 0.5f, 0.25f), waitTime);
  }
}
