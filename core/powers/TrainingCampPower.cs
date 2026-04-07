using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Cards;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers;

/// <summary>
/// The first X Backstage cards you play each turn cost 1 less.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Uncommon.Power.TrainingCamp"/>.
/// </summary>
public class TrainingCampPower() : LinkuraPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  private int _remainingTriggersThisTurn;

  public override Task AfterApplied(Creature applier, CardModel cardSource) {
    _remainingTriggersThisTurn = (int)Amount;
    return base.AfterApplied(applier, cardSource);
  }

  public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player) {
    await base.AfterPlayerTurnStartEarly(choiceContext, player);
    if (player == Owner.Player) {
      Flash();
      _remainingTriggersThisTurn = Amount;
    }
  }

  public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost) {
    modifiedCost = originalCost;
    if (card.Owner.Creature != Owner || _remainingTriggersThisTurn <= 0) return false;
    if (!card.Keywords.Contains(LinkuraKeywords.Backstage)) return false;

    var pileType = card.Pile?.Type;
    if (pileType != PileType.Hand && pileType != PileType.Play) return false;
    if (originalCost <= 0m) return false;

    modifiedCost = System.Math.Max(0m, originalCost - 1m);
    return true;
  }

  public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay) {
    if (cardPlay.Card.Owner.Creature == Owner && _remainingTriggersThisTurn > 0 && cardPlay.Card.Keywords.Contains(LinkuraKeywords.Backstage)) {
      _remainingTriggersThisTurn--;
    }
    return Task.CompletedTask;
  }

}
