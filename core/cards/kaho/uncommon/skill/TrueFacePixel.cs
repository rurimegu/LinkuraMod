using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// True Face Pixel (素颜像素) — Cost 1, Skill, Uncommon.
/// Choose up to 1 (2) cards from the discard pile and add them to your hand.
/// </summary>
public class TrueFacePixel() : KahoCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CardsVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int count = Math.Min(DynamicVars.Cards.IntValue, 10 - PileType.Hand.GetPile(Owner).Cards.Count);
    if (count <= 0) return;
    var discardPile = PileType.Discard.GetPile(Owner).Cards;
    if (discardPile.Count == 0) return;
    var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, count);
    await CardPileCmd.Add(
      await CardSelectCmd.FromSimpleGrid(ctx, discardPile, Owner, prefs),
      PileType.Hand);
  }

  protected override void OnUpgrade() {
    DynamicVars.Cards.UpgradeValueBy(1m);
  }
}
