using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace RuriMegu.Core.Cards.Kaho.Rare.Skill;

/// <summary>
/// Two Seconds Of Eternity — Cost 0 (dynamic), Skill, Rare.
/// Play all other cards in hand. This card's cost equals their total cost.
/// Exhaust. (Upgraded: Remove Exhaust.)
///
/// Guard: tracks total auto-played cards since the last manual play of this card,
/// capped at <see cref="MAX_AUTO_CARDS_PER_PLAY"/>, mirroring InHandTriggerCard's pattern.
/// </summary>
public class TwoSecondsOfEternity() : LinkuraCard(0, CardType.Skill, CardRarity.Rare, TargetType.None) {
  /// <summary>Maximum cards auto-played by this card between consecutive manual plays.</summary>
  private const int MAX_AUTO_CARDS_PER_PLAY = 99;

  private int _autoPlayedCount;

  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

  // ── Dynamic cost tracking ─────────────────────────────────────────────────

  /// <summary>
  /// Recalculate this card's cost whenever any card enters or leaves a pile,
  /// so the displayed cost always equals the sum of the other hand cards' costs.
  /// </summary>
  public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel source) {
    // Only act when a card moved to or from the hand pile.
    bool movedFromHand = oldPileType == PileType.Hand;
    bool movedToHand = card.Pile?.Type == PileType.Hand;
    if (movedFromHand || movedToHand) {
      RecalculateCost();
    }
    return Task.CompletedTask;
  }

  private void RecalculateCost() {
    if (!IsMutable) return;
    var hand = PileType.Hand.GetPile(Owner);
    if (hand == null) return;

    int totalCost = hand.Cards
      .Where(c => c != this)
      .Sum(c => c.EnergyCost.GetWithModifiers(CostModifiers.All));

    EnergyCost.SetCustomBaseCost(totalCost);
  }

  // ── Play effect ───────────────────────────────────────────────────────────

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    // Snapshot hand at this moment, excluding this card (already in Play pile).
    var cardsToPlay = PileType.Hand.GetPile(Owner).Cards
      .Where(c => c != this)
      .ToList();

    foreach (var card in cardsToPlay) {
      if (_autoPlayedCount >= MAX_AUTO_CARDS_PER_PLAY) break;
      // Skip cards that have already left the hand (e.g., played by a prior card in this loop).
      if (card.Pile?.Type != PileType.Hand) continue;
      await CardCmd.AutoPlay(ctx, card, null);
      _autoPlayedCount++;
    }
  }

  // ── Guard reset ───────────────────────────────────────────────────────────

  public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay) {
    await base.AfterCardPlayed(context, cardPlay);
    // Reset the counter only when this card is played manually by the player,
    // matching the InHandTriggerCard.AfterCardPlayed pattern.
    if (cardPlay.Card == this && !cardPlay.IsAutoPlay) {
      _autoPlayedCount = 0;
    }
  }

  public override Task AfterCombatEnd(CombatRoom room) {
    _autoPlayedCount = 0;
    return base.AfterCombatEnd(room);
  }

  // ── Upgrade ───────────────────────────────────────────────────────────────

  protected override void OnUpgrade() {
    RemoveKeyword(CardKeyword.Exhaust);
  }
}
