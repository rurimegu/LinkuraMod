using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Random;
using RuriMegu.Core.Characters;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Patches;

/// <summary>
/// After <see cref="NRestSiteCharacter._Ready"/> sets the act-specific looping
/// animation on every child SpineSprite, remap the animation to the Linkura
/// character's equivalent if the player is using a <see cref="LinkuraCharacterModel"/>.
///
/// The vanilla code runs before this postfix, so all SpineSprites already have the
/// vanilla animation queued; we simply override it with the mapped name.
/// </summary>
[HarmonyPatch(typeof(NRestSiteCharacter), nameof(NRestSiteCharacter._Ready))]
public static class RestSiteCharacterPatch {
  [HarmonyPostfix]
  public static void Postfix(NRestSiteCharacter __instance) {
    if (__instance.Player?.Character is not LinkuraCharacterModel linkuraChara) return;

    string vanillaAnimName = __instance.Player.RunState.CurrentActIndex switch {
      0 => LinkuraAnimation.VANILLA_ANIM_REST_SITE_ACT1,
      1 => LinkuraAnimation.VANILLA_ANIM_REST_SITE_ACT2,
      2 => LinkuraAnimation.VANILLA_ANIM_REST_SITE_ACT3,
      _ => null,
    };
    if (vanillaAnimName is null) return;

    string mappedAnimName = linkuraChara.GetMappedAnimation(vanillaAnimName);

    foreach (Node2D childSpineNode in __instance.GetChildren().OfType<Node2D>()
               .Where(n => n.GetClass() == "SpineSprite")) {
      MegaSprite spine = new(childSpineNode);
      if (!spine.HasAnimation(mappedAnimName)) {
        LinkuraMod.Logger.Warn($"RestSiteCharacterPatch: animation '{mappedAnimName}' not found on SpineSprite");
        continue;
      }
      MegaTrackEntry track = spine.GetAnimationState().SetAnimation(mappedAnimName, loop: true);
      track?.SetTrackTime(track.GetAnimationEnd() * Rng.Chaotic.NextFloat());
    }
  }
}
