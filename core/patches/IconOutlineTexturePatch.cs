using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Characters;

namespace RuriMegu.Core.Patches;

/// <summary>
/// <see cref="CharacterModel.IconOutlineTexture"/> is backed by a private, non-virtual
/// path property, so there is no BaseLib hook for it. This prefix intercepts the getter
/// for any <see cref="LinkuraCharacterModel"/> and redirects to
/// <see cref="LinkuraCharacterModel.CustomIconOutlineTexturePath"/>.
/// </summary>
[HarmonyPatch(typeof(CharacterModel), nameof(CharacterModel.IconOutlineTexture), MethodType.Getter)]
public static class IconOutlineTexturePatch {
  [HarmonyPrefix]
  public static bool Prefix(CharacterModel __instance, ref Texture2D __result) {
    if (__instance is LinkuraCharacterModel linkura) {
      __result = PreloadManager.Cache.GetTexture2D(linkura.CustomIconOutlineTexturePath);
      return false;
    }
    return true;
  }
}
