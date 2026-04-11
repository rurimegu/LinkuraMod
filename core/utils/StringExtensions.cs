using System.IO;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace RuriMegu.Core.Utils;

/// <summary>
/// Utility extension methods for resolving asset paths within the mod.
/// </summary>
public static class StringExtensions {
  public static string ImagePath(this string path, string characterId = "") {
    return Path.Join(LinkuraMod.ModId, "images", characterId, path);
  }

  public static string CardImagePath(this string path, string characterId = "") {
    return Path.Join(LinkuraMod.ModId, "images", "card_portraits", characterId, path);
  }

  public static string BigCardImagePath(this string path, string characterId = "") {
    return Path.Join(LinkuraMod.ModId, "images", "card_portraits", characterId, "big", path);
  }

  public static string PowerImagePath(this string path, string characterId = "") {
    return Path.Join(LinkuraMod.ModId, "images", "powers", characterId, path);
  }

  public static string RelicImagePath(this string path, string characterId = "") {
    return Path.Join(LinkuraMod.ModId, "images", "relics", characterId, path);
  }

  public static string BigRelicImagePath(this string path, string characterId = "") {
    return Path.Join(LinkuraMod.ModId, "images", "relics", characterId, "big", path);
  }

  public static string CharacterUiPath(this string path, string characterId = "") {
    return Path.Join(LinkuraMod.ModId, "images", "charui", characterId, path);
  }

  public static string CharacterScenePath(this string path, string characterId = "") {
    return Path.Join(LinkuraMod.ModId, "scenes", characterId, path);
  }

  public static string RemoveSuffix(this string str, string suffix) {
    return str.EndsWith(suffix) ? str[..^suffix.Length] : str;
  }

  private static LocString L10NStatic(string entry) {
    return new LocString("static_hover_tips", entry);
  }

  public static HoverTip HoverTip(this string locKey, params DynamicVar[] vars) {
    string text = locKey;
    LocString locString = L10NStatic(text + ".title");
    LocString locString2 = L10NStatic(text + ".description");
    foreach (DynamicVar dynamicVar in vars) {
      locString.Add(dynamicVar);
      locString2.Add(dynamicVar);
    }

    return new HoverTip(locString, locString2);
  }
}
