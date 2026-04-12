using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using RuriMegu.Core.Characters;

namespace RuriMegu.Core.Utils;

public static class LinkuraAnimation {
  public const string TRIGGER_NAME_CAST = "Cast";
  public const string ANIM_NAME_BURST = "burst";
  public const string ANIM_NAME_COLLECT = "collect";

  public static async Task PlayCastAnim(this Player player) {
    if (player.Character is LinkuraCharacterModel linkuraChara) {
      await CreatureCmd.TriggerAnim(player.Creature, TRIGGER_NAME_CAST, linkuraChara.CastAnimDelay);
    }
  }

  public static async Task PlayBurstAnim(this Player player) {
    if (player.Character is LinkuraCharacterModel linkuraChara) {
      await PlayCustomSpineAnim(player, linkuraChara.AnimNameBurst, linkuraChara.BurstAnimDelay);
    }
  }

  public static async Task PlayCollectAnim(this Player player) {
    if (player.Character is LinkuraCharacterModel linkuraChara) {
      await PlayCustomSpineAnim(player, linkuraChara.AnimNameCollect, linkuraChara.CollectAnimDelay);
    }
  }

  // CreatureCmd.TriggerAnim goes through CreatureAnimator's state machine, which only fires
  // registered trigger branches. Custom Spine animations (burst/collect) are not in the state
  // machine, so we bypass it and drive the SpineAnimationState directly.
  private static async Task PlayCustomSpineAnim(Player player, string animName, float waitTime) {
    var spine = NCombatRoom.Instance?.GetCreatureNode(player.Creature)?.SpineController;
    if (spine == null || !spine.HasAnimation(animName)) {
      LinkuraMod.Logger.Warn($"Could not play animation '{animName}' - SpineController or animation not found");
      return;
    }

    var animState = spine.GetAnimationState();
    animState.SetAnimation(animName, false);
    animState.AddAnimation(AnimState.idleAnim, 0f, true);

    await Cmd.CustomScaledWait(Mathf.Min(waitTime * 0.5f, 0.25f), waitTime);
  }
}
