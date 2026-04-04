using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Rare.Skill;

/// <summary>
/// Summery Pain — Cost 1, Skill, Rare.
/// Discard your hand and draw that many cards.
/// Backstage: Whenever you have fewer than 6 (8) cards in hand, draw 1 card.
/// </summary>
public class SummeryPain() : InHandTriggerCard(1, CardType.Skill, CardRarity.Rare, TargetType.None) {
  private const string HAND_THRESHOLD_VAR = "RURIMEGU-HAND_THRESHOLD";

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DynamicVar(HAND_THRESHOLD_VAR, 6m),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.DiscardAndDraw(this, ctx);
  }

  public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay play) {
    await base.AfterCardPlayed(ctx, play);
    if (!this.IsInHand() || play.Card == this) return;

    var pile = PileType.Hand.GetPile(Owner);

    while (pile.Cards.Count < DynamicVars[HAND_THRESHOLD_VAR].IntValue) {
      int cardsDrawn = 0;
      await TriggerWithAction(ctx, async () => {
        var drawn = await CardPileCmd.Draw(ctx, 1, Owner);
        cardsDrawn += drawn.Count();
      });
      if (cardsDrawn == 0) break;
    }
  }

  protected override void OnUpgrade() {
    DynamicVars[HAND_THRESHOLD_VAR].UpgradeValueBy(2m);
  }
}
