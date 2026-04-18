using System.Reflection;
using BaseLib.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using RuriMegu.Core.Config;

namespace RuriMegu;

[ModInitializer(nameof(Initialize))]
public static class LinkuraMod {
  public const string ModId = "linkuramod";

  public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
  new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

  public static void Initialize() {
    Logger.Info("Link! Like! LoveLive! - LinkuraMod Initializing...");
    ModConfigRegistry.Register(ModId, new LinkuraModConfig());
    Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());
    Harmony harmony = new(ModId);
    harmony.PatchAll();
  }
}
