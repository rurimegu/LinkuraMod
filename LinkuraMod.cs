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
  public const string ModId = "LinkuraMod";

  public static Logger Logger { get; private set; }

  public static void Initialize() {
    Logger = RitsuLibFramework.CreateLogger(ModId);
    Logger.Info("Link! Like! LoveLive! - LinkuraMod Initializing...");

    Assembly asm = Assembly.GetExecutingAssembly();
    RitsuLibFramework.EnsureGodotScriptsRegistered(asm, Logger);
    ModTypeDiscoveryHub.RegisterModAssembly(ModId, asm);

    // Mod settings
    LinkuraModConfig.RegisterSettings(ModId);

    // Content pack: starting cards (character registered via [RegisterCharacter],
    // keywords via [RegisterOwnedCardKeyword], starter relic via [RegisterCharacterStarterRelic])
    RitsuLibFramework.CreateContentPack(ModId)
      .Entry(new CharacterStarterCardRegistrationEntry<HinoshitaKaho, KahoStrike>(4))
      .Entry(new CharacterStarterCardRegistrationEntry<HinoshitaKaho, KahoDefend>(4))
      .Entry(new CharacterStarterCardRegistrationEntry<HinoshitaKaho, LinkuraEnergy>())
      .Entry(new CharacterStarterCardRegistrationEntry<HinoshitaKaho, WideHeart>())
      .Apply();

    // Patches
    var patcher = RitsuLibFramework.CreatePatcher(ModId, "core-patches");
    BackstageCardPatch.AddTo(patcher);
    HeartCounterPatches.AddTo(patcher);
    LinkuraSkinSyncPatches.AddTo(patcher);
    SpineAnimationPatches.AddTo(patcher);
    patcher.PatchAll();
  }
}
