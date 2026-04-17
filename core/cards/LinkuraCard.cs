using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards;

/// <summary>
/// Base class for all Linkura-pool cards.
/// Provides subscription tracking with self-managed lifecycle:
/// subscriptions are initialized when the card enters combat and
/// disposed when the card leaves all piles or combat ends.
/// No external driver (e.g. a relic) is needed.
/// </summary>
public abstract class LinkuraCard(int cost, CardType type, CardRarity rarity, TargetType target)
  : CustomCardModel(cost, type, rarity, target) {
  public virtual string CharacterId => "";
  public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath(CharacterId);
  public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath(CharacterId);
  public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath(CharacterId);

  private readonly List<Subscription> _subs = [];
  private bool _subscriptionsInitialized;

  /// <summary>
  /// Override to set up subscriptions via TrackSubscription().
  /// Called automatically at BeforeCombatStartLate and AfterCardEnteredCombat.
  /// </summary>
  protected virtual Task InitializeSubscriptions() => Task.CompletedTask;

  /// <summary>Track a subscription for automatic cleanup.</summary>
  protected void TrackSubscription(Subscription sub) => _subs.Add(sub);

  /// <summary>Dispose all tracked subscriptions and reset state.</summary>
  private void DisposeAllSubscriptions() {
    foreach (var sub in _subs) sub.Dispose();
    _subs.Clear();
    _subscriptionsInitialized = false;
  }

  private async Task EnsureSubscriptionsInitialized() {
    if (_subscriptionsInitialized) return;
    _subscriptionsInitialized = true;
    await InitializeSubscriptions();
  }

  // ── Trigger-count guard ────────────────────────────────────────────────

  /// <summary>Maximum times this card's automatic effect may fire per combat.</summary>
  protected virtual int MaxTriggersPerPlay => 999;

  private int _triggerCount;

  /// <summary>
  /// Returns false when combat has ended or the trigger cap has been reached.
  /// Override to add additional conditions (e.g. pile checks for backstage cards).
  /// </summary>
  protected virtual bool CanTrigger() => !CombatManager.Instance.IsOverOrEnding && _triggerCount < MaxTriggersPerPlay;

  protected void IncrementTriggerCount() {
    _triggerCount++;
  }

  // ── Self-managed lifecycle hooks ───────────────────────────────────────

  /// <summary>
  /// Initialize subscriptions for cards already in the deck at combat start.
  /// </summary>
  public override async Task BeforeCombatStartLate() {
    await EnsureSubscriptionsInitialized();
    await base.BeforeCombatStartLate();
  }

  /// <summary>
  /// Initialize subscriptions for cards added mid-combat.
  /// </summary>
  public override async Task AfterCardEnteredCombat(CardModel card) {
    if (card == this)
      await EnsureSubscriptionsInitialized();
    await base.AfterCardEnteredCombat(card);
  }

  /// <summary>
  /// Dispose subscriptions when the card leaves all piles (removed from combat).
  /// </summary>
  public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel source) {
    if (card == this && Pile == null)
      DisposeAllSubscriptions();
    return base.AfterCardChangedPiles(card, oldPileType, source);
  }

  /// <summary>
  /// Dispose subscriptions at end of combat.
  /// </summary>
  public override Task AfterCombatEnd(CombatRoom room) {
    _triggerCount = 0;
    DisposeAllSubscriptions();
    return base.AfterCombatEnd(room);
  }

  public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay) {
    await base.AfterCardPlayed(context, cardPlay);
    // Reset counter only when this card is manually played by the player.
    if (cardPlay.Card == this && !cardPlay.IsAutoPlay) {
      _triggerCount = 0;
    }
  }

  public override Task BeforeSideTurnStart(PlayerChoiceContext ctx, CombatSide side, CombatState combatState) {
    if (side == Owner?.Creature?.Side) {
      _triggerCount = 0;
    }
    return base.BeforeSideTurnStart(ctx, side, combatState);
  }
}
