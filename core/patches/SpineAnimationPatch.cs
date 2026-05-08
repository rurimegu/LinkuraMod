using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using RuriMegu.Core.Characters;

namespace RuriMegu.Core.Patches;

/// <summary>
/// Caches instance IDs of MegaAnimationState objects that belong to Linkura (Kaho) spines.
/// Populated via MegaSprite.GetAnimationState postfix, where HasAnimation is accessible.
/// </summary>
[HarmonyPatch(typeof(MegaSprite), nameof(MegaSprite.GetAnimationState))]
public static class SpineAnimStateCachePatch {
  private const string LINKURA_DETECT_ANIM = "quest_dance_mentaldown";
  internal static readonly HashSet<ulong> LinkuraStateIds = new();

  public static void Postfix(MegaSprite __instance, MegaAnimationState __result) {
    if (__result?.BoundObject != null && __instance.HasAnimation(LINKURA_DETECT_ANIM)) {
      LinkuraStateIds.Add(__result.BoundObject.GetInstanceId());
      LinkuraMod.Logger.Debug($"[SpineAnimationPatch] Registered Linkura animation state id={__result.BoundObject.GetInstanceId()}");
    }
  }
}

[HarmonyPatch(typeof(MegaAnimationState), nameof(MegaAnimationState.SetAnimation))]
public static class SpineAnimationPatch {
  public static void Prefix(MegaAnimationState __instance, ref string animationName) {
    if (SpineAnimStateCachePatch.LinkuraStateIds.Contains(__instance.BoundObject.GetInstanceId())) {
      if (LinkuraCharacterModel.MAPPED_ANIMATIONS.TryGetValue(animationName, out string mappedName)) {
        LinkuraMod.Logger.Debug($"[SpineAnimationPatch] Rewriting '{animationName}' -> '{mappedName}'");
        animationName = mappedName;
      } else {
        LinkuraMod.Logger.Debug($"[SpineAnimationPatch] No mapped animation for '{animationName}', using original.");
      }
    }
  }
}
