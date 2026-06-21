using System;
using System.Reflection;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using RuriMegu.Core.Cards.Kaho.Basic.Attack;
using RuriMegu.Core.Cards.Kaho.Basic.Skill;
using RuriMegu.Core.Characters.Kaho;
using RuriMegu.Core.Config;
using RuriMegu.Core.Patches;
using STS2RitsuLib;
using STS2RitsuLib.Interop;
using STS2RitsuLib.Scaffolding.Content;

namespace RuriMegu;

[ModInitializer(nameof(Initialize))]
public static class LinkuraMod {
  public const string MOD_ID = "LinkuraMod";
  public const long STEAM_WORKSHOP_MOD_ID = 3748068334L;
  public const long STEAM_WORKSHOP_SKIN_MOD_ID = 3748835042L;

  public static Logger Logger { get; private set; }

  public static void Initialize() {
    Logger = RitsuLibFramework.CreateLogger(MOD_ID);
    Logger.Info("Link! Like! LoveLive! - LinkuraMod Initializing...");

    Assembly asm = Assembly.GetExecutingAssembly();
    RitsuLibFramework.EnsureGodotScriptsRegistered(asm, Logger);
    ModTypeDiscoveryHub.RegisterModAssembly(MOD_ID, asm);

    // Mod settings
    LinkuraModConfig.RegisterSettings(MOD_ID);

    // Content pack: starting cards (character registered via [RegisterCharacter],
    // keywords via [RegisterOwnedCardKeyword], starter relic via [RegisterCharacterStarterRelic])
    RitsuLibFramework.CreateContentPack(MOD_ID)
      .Entry(new CharacterStarterCardRegistrationEntry<HinoshitaKaho, KahoStrike>(4))
      .Entry(new CharacterStarterCardRegistrationEntry<HinoshitaKaho, KahoDefend>(4))
      .Entry(new CharacterStarterCardRegistrationEntry<HinoshitaKaho, LinkuraEnergy>())
      .Entry(new CharacterStarterCardRegistrationEntry<HinoshitaKaho, WideHeart>())
      .Apply();

    // Patches
    var patcher = RitsuLibFramework.CreatePatcher(MOD_ID, "core-patches");
    BackstageCardPatch.AddTo(patcher);
    HeartCounterPatches.AddTo(patcher);
    LinkuraSkinSyncPatches.AddTo(patcher);
    SpineAnimationPatches.AddTo(patcher);
    patcher.PatchAll();

    // Update check
    string currentVersion = asm.GetName().Version?.ToString(3) ?? "0.0.0";
    RitsuLibFramework.RegisterModUpdateCheck(RitsuLibFramework.SkipModUpdateCheckWhenLoadedFromSteamWorkshop(new() {
      ModId = MOD_ID,
      DisplayName = "LinkuraMod",
      CurrentVersion = currentVersion,
      ManifestUri = new Uri("https://files.rurino.dev/linkuramod/update.json"),
      ReleasePageUri = new Uri("https://github.com/rurimegu/LinkuraMod/releases"),
    }, typeof(LinkuraMod).Assembly, STEAM_WORKSHOP_MOD_ID));
  }
}

