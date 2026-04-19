using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace RuriMegu.Core.Utils;

/// <summary>
/// Metadata read from <c>rurimegu_spine_metadata.json</c> inside each skin folder.
/// </summary>
public class SkinMetadata {
  [JsonPropertyName("atlas")]
  public string Atlas { get; set; } = "";

  [JsonPropertyName("skel")]
  public string Skel { get; set; } = "";

  [JsonPropertyName("textures")]
  public List<string> Textures { get; set; } = new();

  [JsonPropertyName("characterName")]
  public string CharacterName { get; set; } = "";

  [JsonPropertyName("characterNameJp")]
  public string CharacterNameJp { get; set; } = "";

  [JsonPropertyName("cardName")]
  public string CardName { get; set; } = "";
}

/// <summary>
/// Represents one discovered skin: the folder name used as the config key
/// and the display label shown in the UI.
/// </summary>
public class SkinEntry {
  public string FolderName { get; }
  public string DisplayLabel { get; }

  public SkinEntry(string folderName, string displayLabel) {
    FolderName = folderName;
    DisplayLabel = displayLabel;
  }
}

/// <summary>
/// Discovers and validates external spine skins from <c>&lt;dll_folder&gt;/skins/&lt;skin_folder&gt;/</c>.
/// Each skin folder must contain a <c>rurimegu_spine_metadata.json</c> file specifying the atlas,
/// skel, and display name, and must have all MAPPED_ANIMATIONS present.
/// </summary>
public static class SpineSkinLoader {
  /// <summary>Label / config value representing the built-in bundled spine skin.</summary>
  public const string BUILTIN_SKIN_LABEL = "Built-in";

  private const string SKINS_FOLDER = "skins";
  private const string METADATA_FILENAME = "spine_metadata.rurimegu";
  private const float DEFAULT_MIX = 0.4f;

  private static readonly JsonSerializerOptions JSON_OPTIONS = new() {
    PropertyNameCaseInsensitive = true,
  };

  /// <summary>Returns the directory where the mod DLL is located.</summary>
  public static string DllFolder =>
    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";

  /// <summary>Returns the skins root directory.</summary>
  public static string GetSkinsPath() => Path.Join(DllFolder, SKINS_FOLDER);

  /// <summary>
  /// Returns all valid skins. The first entry is always the built-in skin.
  /// Skins without a valid metadata file or missing required animations are skipped with a warning.
  /// </summary>
  public static List<SkinEntry> GetAvailableSkins() {
    var result = new List<SkinEntry> { new SkinEntry(BUILTIN_SKIN_LABEL, BUILTIN_SKIN_LABEL) };

    var skinsPath = GetSkinsPath();
    if (!Directory.Exists(skinsPath))
      return result;

    foreach (var dir in Directory.EnumerateDirectories(skinsPath)) {
      var folderName = Path.GetFileName(dir);
      var metadata = TryReadMetadata(dir);

      if (metadata == null) {
        LinkuraMod.Logger.Warn($"[SpineSkinLoader] Skin '{folderName}' has no valid metadata — skipped.");
        continue;
      }

      if (!IsValidSkin(dir, metadata)) {
        LinkuraMod.Logger.Warn($"[SpineSkinLoader] Skin '{folderName}' failed validation — skipped.");
        continue;
      }

      var displayLabel = $"{metadata.CharacterName} - {metadata.CardName}";
      result.Add(new SkinEntry(folderName, displayLabel));
    }

    return result;
  }

  /// <summary>
  /// Reads and deserializes <c>rurimegu_spine_metadata.json</c> from the given skin directory.
  /// Returns null if the file is missing or cannot be parsed.
  /// </summary>
  public static SkinMetadata TryReadMetadata(string skinDir) {
    var metaPath = Path.Join(skinDir, METADATA_FILENAME);
    if (!File.Exists(metaPath))
      return null;

    try {
      var json = File.ReadAllText(metaPath);
      return JsonSerializer.Deserialize<SkinMetadata>(json, JSON_OPTIONS);
    } catch (System.Exception e) {
      LinkuraMod.Logger.Warn($"[SpineSkinLoader] Failed to parse metadata at '{metaPath}': {e.Message}");
      return null;
    }
  }

  /// <summary>
  /// Validates that the skin directory has the atlas and skel files on disk as specified in metadata.
  /// Does NOT load skeleton data — that happens lazily in <see cref="LoadSkin"/>.
  /// </summary>
  public static bool IsValidSkin(string skinDir, SkinMetadata metadata) {
    if (string.IsNullOrEmpty(metadata.Atlas) || string.IsNullOrEmpty(metadata.Skel)) {
      LinkuraMod.Logger.Warn($"[SpineSkinLoader] Metadata for '{skinDir}' is missing atlas or skel field.");
      return false;
    }

    var atlasFile = Path.Join(skinDir, metadata.Atlas);
    var skelFile = Path.Join(skinDir, metadata.Skel);

    if (!File.Exists(atlasFile) || !File.Exists(skelFile)) {
      LinkuraMod.Logger.Warn($"[SpineSkinLoader] Skin '{skinDir}' is missing atlas or skel file on disk.");
      return false;
    }

    return true;
  }

  /// <summary>
  /// Attempts to load a <see cref="MegaSkeletonDataResource"/> from external .atlas and .skel files.
  /// Instantiates spine resources via ClassDB (bypassing ResourceLoader, which cannot resolve
  /// absolute filesystem paths for GDExtension resource types).
  /// Returns null if loading fails for any reason.
  /// </summary>
  public static MegaSkeletonDataResource TryLoadSkeletonData(string atlasFilePath, string skelFilePath) {
    try {
      // No C# bindings exist for the spine-godot GDExtension, so we use ClassDB
      // reflection (the standard workaround per the Godot proposals discussion).
      // See: https://esotericsoftware.com/spine-godot#Loading-.skel.json.atlas-files-from-disk

      var skelResObj = ClassDB.Instantiate("SpineSkeletonFileResource").AsGodotObject();
      if (skelResObj == null) {
        LinkuraMod.Logger.Warn("[SpineSkinLoader] Failed to instantiate SpineSkeletonFileResource.");
        return null;
      }
      skelResObj.Call("load_from_file", skelFilePath);

      var atlasResObj = ClassDB.Instantiate("SpineAtlasResource").AsGodotObject();
      if (atlasResObj == null) {
        LinkuraMod.Logger.Warn("[SpineSkinLoader] Failed to instantiate SpineAtlasResource.");
        return null;
      }
      atlasResObj.Call("load_from_atlas_file", atlasFilePath);

      var dataResObj = ClassDB.Instantiate("SpineSkeletonDataResource").AsGodotObject();
      if (dataResObj == null) {
        LinkuraMod.Logger.Warn("[SpineSkinLoader] Failed to instantiate SpineSkeletonDataResource.");
        return null;
      }
      dataResObj.Set("skeleton_file_res", Variant.From(skelResObj));
      dataResObj.Set("atlas_res", Variant.From(atlasResObj));
      dataResObj.Set("default_mix", DEFAULT_MIX);

      return new MegaSkeletonDataResource(Variant.From(dataResObj));
    } catch (System.Exception e) {
      LinkuraMod.Logger.Error($"[SpineSkinLoader] Exception loading skeleton data: {e.Message}");
      return null;
    }
  }

  /// <summary>
  /// Loads a <see cref="MegaSkeletonDataResource"/> for the named skin (folder name).
  /// Returns null if the skin is built-in, not found, or fails to load.
  /// </summary>
  public static MegaSkeletonDataResource LoadSkin(string skinName) {
    if (string.IsNullOrEmpty(skinName) || skinName == BUILTIN_SKIN_LABEL)
      return null;

    var skinDir = Path.Join(GetSkinsPath(), skinName);
    if (!Directory.Exists(skinDir)) {
      LinkuraMod.Logger.Warn($"[SpineSkinLoader] Skin directory not found: '{skinDir}'.");
      return null;
    }

    var metadata = TryReadMetadata(skinDir);
    if (metadata == null) {
      LinkuraMod.Logger.Warn($"[SpineSkinLoader] No valid metadata for skin '{skinName}'.");
      return null;
    }

    var atlasFile = Path.Join(skinDir, metadata.Atlas);
    var skelFile = Path.Join(skinDir, metadata.Skel);

    return TryLoadSkeletonData(atlasFile, skelFile);
  }

  public static void SwapSkin(string skinName, MegaSprite targetSprite) {
    if (string.IsNullOrEmpty(skinName) || skinName == BUILTIN_SKIN_LABEL)
      return;

    var data = LoadSkin(skinName);
    if (data == null) {
      LinkuraMod.Logger.Error($"[SwapSkin] Failed to load skin '{skinName}'. Using built-in.");
      return;
    }

    targetSprite.SetSkeletonDataRes(data);
    LinkuraMod.Logger.Info($"[SwapSkin] Swapped spine to '{skinName}'.");
  }
}
