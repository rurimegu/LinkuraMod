using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace RuriMegu.Core.Utils;

public static class LinkuraCmd {
  public static async Task IncreaseMaxHearts(Player player, int amount, CardModel source = null) {
    if (amount <= 0) return;
    var ev = new Events.IncreaseMaxHeartsEvent(player, amount, source);
    if (!await Events.IncreaseMaxHearts.InvokeAllEarly(ev)) return;
    var childEv = await HeartsState.AddMaxHearts(player, amount, source);
    if (childEv.IsNullOrCancelled()) return;
    ev.ActualAmount = childEv.NewMaxHearts - childEv.OldMaxHearts;
    await Events.IncreaseMaxHearts.InvokeAllLate(ev);
  }

  public static async Task BurstHearts(Player player, int amount, CardModel source = null) {
    if (amount <= 0) return;
    var ev = new Events.BurstHeartsEvent(player, amount, source);
    if (!await Events.BurstHearts.InvokeAllEarly(ev)) return;
    var childEv = await HeartsState.AddHearts(player, amount, source);
    if (childEv.IsNullOrCancelled()) return;
    ev.ActualAmount = childEv.NewHearts - childEv.OldHearts;
    await Events.BurstHearts.InvokeAllLate(ev);
  }

  public static async Task CollectHearts(Player player, PlayerChoiceContext context, CardModel source = null, Creature target = null, int triggers = 1) {
    int hearts = HeartsState.GetHearts(player);
    if (hearts <= 0) return;
    var ev = new Events.CollectHeartsEvent(player, source);
    if (!await Events.CollectHearts.InvokeAllEarly(ev)) return;
    var targets = await ApplyHeartDamage(hearts, target, player, context, triggers);
    var childEv = await HeartsState.SetHearts(player, 0, source);
    if (childEv.IsNullOrCancelled()) return;
    ev.Amount = hearts;
    ev.Targets = targets;
    await Events.CollectHearts.InvokeAllLate(ev);
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
}
