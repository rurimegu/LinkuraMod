using System.Collections.Generic;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using RuriMegu.Core.Characters;
using STS2RitsuLib.Patching.Core;
using STS2RitsuLib.Patching.Models;

namespace RuriMegu.Core.Patches;

public class SpineAnimationPatches : IModPatches {
  public static void AddTo(ModPatcher patcher) {
    patcher.RegisterPatch<SpineAnimStateCachePatch>();
    patcher.RegisterPatch<SpineAnimationPatch>();
  }
}

public class SpineAnimStateCachePatch : IPatchMethod {
  public static string PatchId => "spine_anim_state_cache";
  public static string Description => "Cache MegaAnimationState IDs that belong to Linkura (Kaho) spines";
  public static bool IsCritical => false;

  private const string LINKURA_DETECT_ANIM = "quest_dance_mentaldown";
  internal static readonly HashSet<ulong> LinkuraStateIds = new();

  public static ModPatchTarget[] GetTargets() =>
    [new(typeof(MegaSprite), nameof(MegaSprite.GetAnimationState))];

  public static void Postfix(MegaSprite __instance, MegaAnimationState __result) {
    if (__result?.BoundObject != null && __instance.HasAnimation(LINKURA_DETECT_ANIM)) {
      LinkuraStateIds.Add(__result.BoundObject.GetInstanceId());
      LinkuraMod.Logger.Debug($"[SpineAnimationPatch] Registered Linkura animation state id={__result.BoundObject.GetInstanceId()}");
    }
  }
}

public class SpineAnimationPatch : IPatchMethod {
  public static string PatchId => "spine_animation_remap";
  public static string Description => "Remap vanilla animation names to Kaho-specific animation names for Linkura spines";
  public static bool IsCritical => false;

  public static ModPatchTarget[] GetTargets() =>
    [new(typeof(MegaAnimationState), nameof(MegaAnimationState.SetAnimation))];

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
