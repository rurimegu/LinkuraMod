using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Power;

/// <summary>
/// Career Survey (进路调查) — Cost 1, Power, Uncommon.
/// Whenever you burst at least 8 (6) hearts in a single instance and don't reach maximum, draw 1 card.
/// </summary>
public class CareerSurvey() : LinkuraCard(1, CardType.Power, CardRarity.Uncommon, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CardsVar(1),
    new BurstHeartsVar(8),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    if (IsUpgraded) {
      await PowerCmd.Apply<CareerSurveyUpgradedPower>(Owner.Creature, DynamicVars.Cards.IntValue, Owner.Creature, this);
    } else {
      await PowerCmd.Apply<CareerSurveyPower>(Owner.Creature, DynamicVars.Cards.IntValue, Owner.Creature, this);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.BurstHearts().UpgradeValueBy(-2m);
  }
}
