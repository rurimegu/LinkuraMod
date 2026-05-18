using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace RuriMegu.Core.Cards.Kaho.Rare.Skill;

/// <summary>
/// Enduring Tradition — Cost 1, Skill, Rare.
/// Choose 1 card in your hand and transform it into Enduring Tradition
/// (Upgraded: Enduring Tradition+). Draw 1 (2) card(s). Ethereal.
/// Cannot be played if it is the only card in hand.
/// </summary>
public class EnduringTradition() : KahoCard(1, CardType.Skill, CardRarity.Rare, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CardsVar(1),
  ];

  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CardPileCmd.Draw(ctx, DynamicVars.Cards.IntValue, Owner);

    var hand = PileType.Hand.GetPile(Owner);
    if (hand == null || !hand.Cards.Any()) return;

    var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
    var selected = (await CardSelectCmd.FromHand(ctx, Owner, prefs, null, this)).FirstOrDefault();
    if (selected == null) return;

    var replacement = CombatState.CreateCard<EnduringTradition>(Owner);
    if (IsUpgraded) {
      CardCmd.Upgrade(replacement);
    }
    await CardCmd.Transform(selected, replacement);
  }

  protected override void OnUpgrade() {
    DynamicVars.Cards.UpgradeValueBy(1m);
  }
}
