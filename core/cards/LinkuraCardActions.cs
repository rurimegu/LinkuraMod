using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards;

public static class LinkuraCardActions {
  public static Task IncreaseMaxHearts(CardModel card) {
    return LinkuraCmd.IncreaseMaxHearts(card.Owner, card.DynamicVars.ExpandHearts().IntValue, card);
  }

  public static Task BurstHearts(CardModel card) {
    return LinkuraCmd.BurstHearts(card.Owner, card.DynamicVars.BurstHearts().IntValue, card);
  }

  public static Task CollectHearts(CardModel card, PlayerChoiceContext context, Creature target = null) {
    return LinkuraCmd.CollectHearts(card.Owner, context, card, target);
  }
}
