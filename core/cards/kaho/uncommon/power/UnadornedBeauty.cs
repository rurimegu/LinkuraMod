using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Powers.Kaho;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Power;

/// <summary>
/// Unadorned Beauty (无饰的美丽) — Cost 2, Power, Uncommon.
/// Start of turn gain 1 (2) Energy. Cannot increase max ❤️. Retain.
/// </summary>
public class UnadornedBeauty() : KahoCard(2, CardType.Power, CardRarity.Uncommon, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new EnergyVar(1)
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await Owner.PlayCastAnim();
    await PowerCmd.Apply<UnadornedBeautyPower>(ctx, Owner.Creature, DynamicVars.Energy.IntValue, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.Energy.UpgradeValueBy(1);
  }
}
