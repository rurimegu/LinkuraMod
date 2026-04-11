using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace RuriMegu.Core.Powers.Kaho;

/// <summary>
/// Reduces the cost of ALL Skill cards by <c>Amount</c> while they are in Hand or Play pile.
/// This covers cards drawn or generated after the effect is applied.
/// Consumed (removed entirely) the first time any Skill card is played.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Common.Attack.Fantasy375"/>.
/// </summary>
public class SkillCostReductionPower : KahoPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost) {
    modifiedCost = originalCost;
    if (card.Owner.Creature != Owner) return false;
    if (card.Type != CardType.Skill) return false;
    var pileType = card.Pile?.Type;
    if (pileType != PileType.Hand && pileType != PileType.Play) return false;
    if (originalCost <= 0m) return false;
    modifiedCost = System.Math.Max(0m, originalCost - Amount);
    return true;
  }

  public override async Task BeforeCardPlayed(CardPlay cardPlay) {
    // Consume this power the moment any Skill card belonging to this owner is played.
    if (cardPlay.Card.Owner.Creature != Owner) return;
    if (cardPlay.Card.Type != CardType.Skill) return;
    var pileType = cardPlay.Card.Pile?.Type;
    if (pileType != PileType.Hand && pileType != PileType.Play) return;
    await PowerCmd.Remove(this);
  }
}
