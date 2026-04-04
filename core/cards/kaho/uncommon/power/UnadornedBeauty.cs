using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Power;

/// <summary>
/// Unadorned Beauty (无饰的美丽) — Cost 2 (1), Power, Uncommon.
/// Start of turn gain 1 Energy. Cannot increase max ❤️.
/// </summary>
public class UnadornedBeauty() : LinkuraCard(2, CardType.Power, CardRarity.Uncommon, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new EnergyVar(1)
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await PowerCmd.Apply<UnadornedBeautyPower>(Owner.Creature, DynamicVars.Energy.IntValue, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    EnergyCost.UpgradeBy(-1);
  }
}
