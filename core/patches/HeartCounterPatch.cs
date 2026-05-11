using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using RuriMegu.Nodes.Combat;
using STS2RitsuLib.Patching.Core;
using STS2RitsuLib.Patching.Models;

namespace RuriMegu.Core.Patches;

public class HeartCounterPatches : IModPatches {
  public static void AddTo(ModPatcher patcher) {
    patcher.RegisterPatch<HeartCounterPatch>();
  }
}

public class HeartCounterPatch : IPatchMethod {
  public static string PatchId => "heart_counter_init";
  public static string Description => "Initialize NHeartCounter when NCombatUi activates";
  public static bool IsCritical => false;

  public static ModPatchTarget[] GetTargets() =>
    [new(typeof(NCombatUi), nameof(NCombatUi.Activate))];

  public static void Postfix(NCombatUi __instance, CombatState state) {
    Player me = LocalContext.GetMe(state);
    LinkuraMod.Logger.Info("[HeartCounterPatch] Postfix for player: " + me.NetId);

    Control energyCounterContainer = __instance.EnergyCounterContainer;
    NHeartCounter heartCounter = null;

    foreach (Node child in energyCounterContainer.GetChildren()) {
      Node found = child.FindChild("HeartCounter", owned: false);
      if (found is NHeartCounter hc) { heartCounter = hc; break; }
    }

    if (heartCounter is null) {
      LinkuraMod.Logger.Warn(
        "[HeartCounterPatch] HeartCounter not found inside energy counter - " +
        "check that energy_counter.tscn contains a HeartCounter child.");
      return;
    }

    heartCounter.Initialize(me);
  }
}
