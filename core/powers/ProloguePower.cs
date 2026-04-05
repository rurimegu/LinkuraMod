using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers;

/// <summary>
/// Whenever Max ❤️ changes, the next card played this turn costs {Amount:energyIcons()} less.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Rare.Power.Prologue"/>.
/// </summary>
public class ProloguePower : LinkuraPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  private int _pendingDiscounts = 0;

  public override Task AfterApplied(Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    TrackSubscription(Events.MaxHeartsChanged.SubscribeLate(OnMaxHeartsChangedLate));
    return base.AfterApplied(applier, cardSource);
  }

  public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState) {
    await base.BeforeSideTurnStart(choiceContext, side, combatState);
    if (side == Owner.Side) {
      _pendingDiscounts = 0;
    }
  }

  public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost) {
    modifiedCost = originalCost;
    if (card.Owner.Creature != Owner) return false;
    if (_pendingDiscounts <= 0) return false;
    var pileType = card.Pile?.Type;
    if (pileType != PileType.Hand && pileType != PileType.Play) return false;
    if (originalCost <= 0m) return false;
    modifiedCost = System.Math.Max(0m, originalCost - Amount);
    return true;
  }

  public override async Task BeforeCardPlayed(CardPlay cardPlay) {
    await base.BeforeCardPlayed(cardPlay);
    if (cardPlay.Card.Owner.Creature != Owner) return;
    if (_pendingDiscounts <= 0) return;
    var pileType = cardPlay.Card.Pile?.Type;
    if (pileType != PileType.Hand && pileType != PileType.Play) return;
    _pendingDiscounts--;
  }

  private Task OnMaxHeartsChangedLate(Events.MaxHeartsChangedEvent ev) {
    if (ev.Player.Creature != Owner || ev.NewMaxHearts == ev.OldMaxHearts) return Task.CompletedTask;
    _pendingDiscounts++;
    Flash();
    return Task.CompletedTask;
  }
}
