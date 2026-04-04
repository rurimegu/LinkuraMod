using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;
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

  /// <summary>Maximum times this card's in-hand effect may fire per player card play.</summary>
  protected virtual int MaxTriggersPerPlay => 999;

  public override IEnumerable<CardKeyword> CanonicalKeywords => [LinkuraKeywords.Backstage];

  private int _triggerCount;

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
    _triggerCount++;
    await Events.TriggerBackstage.InvokeAllLate(ev);
  }

  protected virtual bool CanTrigger() {
    if (_triggerCount >= MaxTriggersPerPlay) return false;
    if (!this.IsInHand()) return false;
    return true;
  }

  public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay) {
    await base.AfterCardPlayed(context, cardPlay);
    // Reset counter only when this card is manually played by the player.
    if (cardPlay.Card == this && !cardPlay.IsAutoPlay) {
      _triggerCount = 0;
    }
  }

  public override Task AfterCombatEnd(CombatRoom room) {
    _triggerCount = 0;
    return base.AfterCombatEnd(room);
  }
}
