using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Characters;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Powers.Kaho;

namespace RuriMegu.Core.Utils;

public static class LinkuraCmd {
  private const int MAX_AUTO_BURST = 9999;

  public static async Task<Events.IncreaseMaxHeartsEvent> IncreaseMaxHearts(Player player, PlayerChoiceContext ctx, int amount, CardModel source = null) {
    var ev = new Events.IncreaseMaxHeartsEvent(player, ctx, amount, source);
    if (!await Events.IncreaseMaxHearts.InvokeAllEarly(ev)) return ev;
    if (amount <= 0) return ev;
    var childEv = await HeartsState.AddMaxHearts(player, ctx, amount, source);
    if (childEv.IsNullOrCancelled()) return ev;
    ev.ActualAmount = childEv.NewMaxHearts - childEv.OldMaxHearts;
    await Events.IncreaseMaxHearts.InvokeAllLate(ev);
    return ev;
  }

  public static async Task GainAutoBurst(Creature creature, int amount, Creature applier, CardModel source) {
    int current = creature.GetPowerAmount<AutoBurstPower>();
    int capped = Math.Min(amount, MAX_AUTO_BURST - current);
    if (capped <= 0) return;
    await PowerCmd.Apply<AutoBurstPower>(creature, capped, applier, source);
  }

  public static async Task<Events.AutoBurstEvent> TriggerAutoBurst(Player player, PlayerChoiceContext ctx, CardModel source = null) {
    int baseAmount = player.Creature.GetPowerAmount<AutoBurstPower>();
    var ev = new Events.AutoBurstEvent(player, ctx, baseAmount, source);
    if (!await Events.AutoBurst.InvokeAllEarly(ev)) return ev;
    var burstEv = await BurstHearts(player, ctx, baseAmount, source, isAutoBurst: true);
    ev.BurstEvent = burstEv;
    if (burstEv.IsNullOrCancelled()) return ev;
    await Events.AutoBurst.InvokeAllLate(ev);
    return ev;
  }

  public static async Task<Events.BurstEvent> BurstHearts(Player player, PlayerChoiceContext ctx, int amount, CardModel source = null, bool isAutoBurst = false) {
    var ev = new Events.BurstEvent(player, ctx, amount, source, isAutoBurst);
    if (!await Events.Burst.InvokeAllEarly(ev)) return ev;
    if (amount <= 0) return ev;
    if (!isAutoBurst) {
      await player.PlayBurstAnim();
    }
    var childEv = await HeartsState.AddHearts(player, ctx, amount, source);
    ev.HeartsChangedEvent = childEv;
    if (childEv.IsNullOrCancelled()) return ev;
    ev.ActualAmount = childEv.NewHearts - childEv.OldHearts;
    await Events.Burst.InvokeAllLate(ev);
    return ev;
  }

  public static async Task<Events.CollectEvent> CollectHearts(Player player, PlayerChoiceContext context, CardModel source = null, Creature target = null, int triggers = 1, bool damageAllEnemies = false) {
    int hearts = Math.Min(HeartsState.GetHearts(player), HeartsState.GetMaxHearts(player));
    damageAllEnemies |= player.HasPower<SpecialAppealPower>();
    var ev = new Events.CollectEvent(player, context, source) {
      DamageAllEnemies = damageAllEnemies
    };
    // Pre-resolve targets before Early so visual effects (e.g. flying hearts) can start
    // animating toward the correct destination before damage numbers appear.
    if (hearts > 0) {
      ev.Targets = ev.DamageAllEnemies
        ? player.Creature.CombatState.HittableEnemies
        : PickTargets(target, player, triggers);
    }
    await player.PlayCollectAnim();
    if (!await Events.Collect.InvokeAllEarly(ev)) return ev;
    if (hearts <= 0) return ev;
    // Apply damage to the pre-resolved (and possibly Early-modified) target list.
    if (ev.Targets?.Count > 0) {
      await CreatureCmd.Damage(context, ev.Targets, hearts, ValueProp.Unpowered, player.Creature);
    }
    var childEv = await HeartsState.SetHearts(player, context, 0, source);
    if (childEv.IsNullOrCancelled()) return ev;
    ev.Amount = hearts;
    await Events.Collect.InvokeAllLate(ev);
    return ev;
  }

  private static IReadOnlyList<Creature> PickTargets(Creature target, Player player, int triggers) {
    var hittable = player.Creature.CombatState.HittableEnemies;
    if (hittable.Count == 0) return [];
    var targets = new List<Creature>();
    if (target != null) { targets.Add(target); triggers--; }
    for (int i = 0; i < triggers; i++) {
      targets.Add(player.RunState.Rng.CombatTargets.NextItem(hittable));
    }
    return targets;
  }

  /// <summary>
  /// Waits for <paramref name="seconds"/> of real wall-clock time, unaffected by
  /// <see cref="Engine.TimeScale"/>, fast mode, or instant mode.
  /// Fires the continuation on the main thread via Godot's SceneTree.
  /// </summary>
  public static Task WaitRealSeconds(float seconds, CancellationToken ct = default) {
    SceneTree sceneTree = (SceneTree)Engine.GetMainLoop();
    SceneTreeTimer timer = sceneTree.CreateTimer(seconds, ignoreTimeScale: true);
    TaskCompletionSource tcs = new();
    timer.Timeout += Receive;
    if (ct.CanBeCanceled)
      ct.Register(() => tcs.TrySetCanceled(ct));
    return tcs.Task;

    void Receive() {
      tcs.TrySetResult();
      timer.Timeout -= Receive;
    }
  }
}
