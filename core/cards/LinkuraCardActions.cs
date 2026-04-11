using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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

  public static Task CollectHearts(CardModel card, PlayerChoiceContext context, Creature target = null, int triggers = 1, bool damageAllEnemies = false) {
    return LinkuraCmd.CollectHearts(card.Owner, context, card, target, triggers, damageAllEnemies);
  }

  public static async Task AutoBurst(CardModel card, PlayerChoiceContext context, int triggers) {
    for (int i = 0; i < triggers; i++) {
      await LinkuraCmd.TriggerAutoBurst(card.Owner, context, card);
    }
  }

  public static Task AutoBurst(CardModel card, PlayerChoiceContext context) {
    return AutoBurst(card, context, card.DynamicVars.TriggerAutoBurst().IntValue);
  }

  public static async Task DiscardAndDraw(CardModel card, PlayerChoiceContext ctx) {
    if (card.Owner == null) return;
    var hand = PileType.Hand.GetPile(card.Owner).Cards;
    int amount = hand.Count;
    if (amount > 0) {
      await CardCmd.Discard(ctx, hand);
      await CardPileCmd.Draw(ctx, amount, card.Owner);
    }
  }
}
