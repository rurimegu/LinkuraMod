using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using RuriMegu.Core.Characters;

namespace RuriMegu.Core.Patches;

[HarmonyPatch(typeof(SpineAnimationAccess), nameof(SpineAnimationAccess.SetAnimation))]
public static class SpineAnimationPatch {
  private static readonly FieldInfo SpriteField = AccessTools.Field(typeof(SpineAnimationAccess), "_sprite");
  private const string LINKURA_DETECT_ANIM = "quest_dance_mentaldown";

  public static void Prefix(SpineAnimationAccess __instance, ref string name) {
    var sprite = (MegaSprite)SpriteField.GetValue(__instance);
    if (sprite?.HasAnimation(LINKURA_DETECT_ANIM) == true) {
      if (LinkuraCharacterModel.MAPPED_ANIMATIONS.TryGetValue(name, out string mappedName)) {
        LinkuraMod.Logger.Info($"Linkura animation rewrite: {name} -> {mappedName}");
        name = mappedName;
      } else {
        LinkuraMod.Logger.Info($"No mapped Linkura animation found for {name}, using original.");
      }
    }
  }
}
