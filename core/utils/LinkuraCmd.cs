using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Powers;

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
    var burstEv = await BurstHearts(player, ctx, baseAmount, source);
    ev.BurstEvent = burstEv;
    if (burstEv.IsNullOrCancelled()) return ev;
    await Events.AutoBurst.InvokeAllLate(ev);
    return ev;
  }

  public static async Task<Events.BurstEvent> BurstHearts(Player player, PlayerChoiceContext ctx, int amount, CardModel source = null) {
    var ev = new Events.BurstEvent(player, ctx, amount, source);
    if (!await Events.Burst.InvokeAllEarly(ev)) return ev;
    if (amount <= 0) return ev;
    var childEv = await HeartsState.AddHearts(player, ctx, amount, source);
    ev.HeartsChangedEvent = childEv;
    if (childEv.IsNullOrCancelled()) return ev;
    ev.ActualAmount = childEv.NewHearts - childEv.OldHearts;
    await Events.Burst.InvokeAllLate(ev);
    return ev;
  }

  public static async Task<Events.CollectEvent> CollectHearts(Player player, PlayerChoiceContext context, CardModel source = null, Creature target = null, int triggers = 1) {
    int hearts = Math.Min(HeartsState.GetHearts(player), HeartsState.GetMaxHearts(player));
    var ev = new Events.CollectEvent(player, context, source);
    if (!await Events.Collect.InvokeAllEarly(ev)) return ev;
    if (hearts <= 0) return ev;
    var targets = ev.DamageAllEnemies
      ? await ApplyHeartDamageAll(hearts, player, context)
      : await ApplyHeartDamage(hearts, target, player, context, triggers);
    var childEv = await HeartsState.SetHearts(player, context, 0, source);
    if (childEv.IsNullOrCancelled()) return ev;
    ev.Amount = hearts;
    ev.Targets = targets;
    await Events.Collect.InvokeAllLate(ev);
    return ev;
  }

  private static async Task<IReadOnlyList<Creature>> ApplyHeartDamage(int value, Creature target, Player player, PlayerChoiceContext choiceContext, int triggers) {
    List<Creature> list = [.. from e in player.Creature.CombatState.GetOpponentsOf(player.Creature)
                           where e.IsHittable
                           select e];
    if (list.Count == 0) {
      return [];
    }
    List<Creature> targets = [];
    if (target != null) {
      targets.Add(target);
      triggers--;
    }
    for (int i = 0; i < triggers; i++) {
      targets.Add(player.RunState.Rng.CombatTargets.NextItem(list));
    }
    await CreatureCmd.Damage(choiceContext, targets, value, ValueProp.Unpowered, player.Creature);
    return targets;
  }

  private static async Task<IReadOnlyList<Creature>> ApplyHeartDamageAll(int value, Player player, PlayerChoiceContext choiceContext) {
    List<Creature> list = [.. from e in player.Creature.CombatState.GetOpponentsOf(player.Creature)
                           where e.IsHittable
                           select e];
    if (list.Count == 0) {
      return [];
    }
    await CreatureCmd.Damage(choiceContext, list, value, ValueProp.Unpowered, player.Creature);
    return list;
  }
}
