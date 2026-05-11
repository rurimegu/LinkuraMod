using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Ancient;

/// <summary>
/// Blooming Heart (绽放之心) — Cost 0, Skill, Ancient (Transcendence upgrade of Wide Heart).
/// Increase max ❤️ by 4 (6). Draw 2 (3) cards; they each cost 1 less next time played.
/// </summary>
public class BloomingHeart() : KahoCard(0, CardType.Skill, CardRarity.Ancient, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new ExpandHeartsVar(4),
    new CardsVar(2),
    new EnergyVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.IncreaseMaxHearts(this, ctx);
    var cards = await CommonActions.Draw(this, ctx);
    foreach (var card in cards) {
      card.EnergyCost.AddUntilPlayed(-1, true);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.ExpandHearts().UpgradeValueBy(2m);
    DynamicVars.Cards.UpgradeValueBy(1m);
  }
}
