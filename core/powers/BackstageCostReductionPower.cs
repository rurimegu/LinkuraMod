using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Cards;

namespace RuriMegu.Core.Powers;

/// <summary>
/// Reduces the cost of the next Backstage card by <c>Amount</c> while in Hand or Play.
/// Consumed when a Backstage card is played.
/// </summary>
public class BackstageCostReductionPower : LinkuraPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost) {
    modifiedCost = originalCost;
    if (card.Owner.Creature != Owner) return false;
    if (!card.Keywords.Contains(LinkuraKeywords.Backstage)) return false;

    var pileType = card.Pile?.Type;
    if (pileType != PileType.Hand && pileType != PileType.Play) return false;
    if (originalCost <= 0m) return false;

    modifiedCost = System.Math.Max(0m, originalCost - Amount);
    return true;
  }

  public override async Task BeforeCardPlayed(CardPlay cardPlay) {
    if (cardPlay.Card.Owner.Creature != Owner) return;
    if (!cardPlay.Card.Keywords.Contains(LinkuraKeywords.Backstage)) return;

    var pileType = cardPlay.Card.Pile?.Type;
    if (pileType != PileType.Hand && pileType != PileType.Play) return;

    await PowerCmd.Remove(this);
  }
}
