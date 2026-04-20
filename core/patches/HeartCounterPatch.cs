using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using RuriMegu.Nodes.Combat;

namespace RuriMegu.Core.Patches;

/// <summary>
/// After <see cref="NCombatUi.Activate"/> runs and adds the energy counter to the
/// EnergyCounterContainer, we look for the <see cref="NHeartCounter"/> node that lives
/// inside the Kaho energy counter scene and call <c>Initialize(player)</c> on it.
///
/// This is safe for non-Kaho characters — if the energy counter has no HeartCounter
/// child the lookup returns null and we bail out.
/// </summary>
[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
public static class HeartCounterPatch {
  [HarmonyPostfix]
  public static void Postfix(NCombatUi __instance, CombatState state) {
    Player me = LocalContext.GetMe(state);
    LinkuraMod.Logger.Info("[HeartCounterPatch] Postfix for player: " + me.NetId);

    // The energy counter was added to EnergyCounterContainer; find the embedded HeartCounter.
    // After BaseLib's NEnergyCounterFactory re-parents nodes the unique-name owner is lost,
    // so we use FindChild + GetNodeOrNull instead of the %UniqueNameLookup.
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
