using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace RuriMegu.Core.Powers.Kaho;

/// <summary>
/// Reduces the cost of the NEXT Power card by <c>Amount</c> while it is in the Hand or Play pile.
/// Consumed (removed entirely) the first time any Power card is played, or at end of turn.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Common.Attack.Fantasy375"/>.
/// </summary>
public class PowerCostReductionPower : KahoPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost) {
    modifiedCost = originalCost;
    if (card.Owner.Creature != Owner) return false;
    if (card.Type != CardType.Power) return false;
    var pileType = card.Pile?.Type;
    if (pileType != PileType.Hand && pileType != PileType.Play) return false;
    if (originalCost <= 0m) return false;
    modifiedCost = System.Math.Max(0m, originalCost - Amount);
    return true;
  }

  public override async Task BeforeCardPlayed(CardPlay cardPlay) {
    // Consume this power the moment any Power card belonging to this owner is played.
    if (cardPlay.Card.Owner.Creature != Owner) return;
    if (cardPlay.Card.Type != CardType.Power) return;
    var pileType = cardPlay.Card.Pile?.Type;
    if (pileType != PileType.Hand && pileType != PileType.Play) return;
    await PowerCmd.Remove(this);
  }

  public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side) {
    await base.AfterTurnEnd(choiceContext, side);
    if (side == Owner.Side) {
      await PowerCmd.Remove(this);
    }
  }
}
