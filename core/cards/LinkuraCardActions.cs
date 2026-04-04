using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards;

public static class LinkuraCardActions {
  public static Task IncreaseMaxHearts(CardModel card, PlayerChoiceContext ctx) {
    return LinkuraCmd.IncreaseMaxHearts(card.Owner, ctx, card.DynamicVars.ExpandHearts().IntValue, card);
  }

  public static Task BurstHearts(CardModel card, PlayerChoiceContext ctx) {
    return LinkuraCmd.BurstHearts(card.Owner, ctx, card.DynamicVars.BurstHearts().IntValue, card);
  }

  public static Task CollectHearts(CardModel card, PlayerChoiceContext context, Creature target = null, int triggers = 1) {
    return LinkuraCmd.CollectHearts(card.Owner, context, card, target, triggers);
  }

  public static async Task AutoBurst(CardModel card, PlayerChoiceContext context, int triggers) {
    for (int i = 0; i < triggers; i++) {
      await LinkuraCmd.TriggerAutoBurst(card.Owner, context, card);
    }
  }

  public static Task AutoBurst(CardModel card, PlayerChoiceContext context) {
    return AutoBurst(card, context, card.DynamicVars.TriggerAutoBurst().IntValue);
  }
}
