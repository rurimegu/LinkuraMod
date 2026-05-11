using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using RuriMegu.Core.Powers.Kaho;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards;

/// <summary>
/// Base class for cards whose effect triggers when a certain condition is met
/// while the card is in the player's hand.
///
/// A guard of <see cref="MaxTriggersPerPlay"/> prevents runaway loops.
/// </summary>
public abstract class InHandTriggerCard(int cost, CardType type, CardRarity rarity, TargetType target)
  : LinkuraCard(cost, type, rarity, target) {

  protected override IEnumerable<string> RegisteredKeywordIds => [LinkuraKeywords.Backstage];

  private async Task<Events.TriggerBackstageEvent> TryTrigger(PlayerChoiceContext ctx) {
    if (!CanTrigger()) return null;
    var ev = new Events.TriggerBackstageEvent(Owner, ctx, this);
    if (!await Events.TriggerBackstage.InvokeAllEarly(ev)) return ev;
    return ev;
  }

  /// <summary>
  /// Triggers the backstage effect, handling repetitions from powers like <c>HolidayHolidayPower</c>.
  /// </summary>
  protected async Task<Events.TriggerBackstageEvent> TriggerWithAction(PlayerChoiceContext ctx, Func<Task> action) {
    var ev = await TryTrigger(ctx);
    if (ev == null || ev.IsNullOrCancelled()) return ev;

    for (int i = 0; i <= ev.RepeatCount; i++) {
      await action();
    }
    await AfterTrigger(ev);
    return ev;
  }

  private async Task AfterTrigger(Events.TriggerBackstageEvent ev) {
    IncrementTriggerCount();
    await Events.TriggerBackstage.InvokeAllLate(ev);
  }

  protected override bool CanTrigger() {
    if (!base.CanTrigger()) return false;
    if (this.IsInHand()) return true;
    if (this.IsInDiscardPile() && (Owner?.Creature?.HasPower<BloomGardenPartyPower>() == true)) return true;
    return false;
  }
}
