using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Powers.Kaho;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Power;

/// <summary>
/// Unadorned Beauty (鏃犻グ鐨勭編涓? 鈥?Cost 2, Power, Uncommon.
/// Start of turn gain 1 (2) Energy. Cannot increase max 鉂わ笍. Retain.
/// </summary>
public class UnadornedBeauty() : KahoCard(2, CardType.Power, CardRarity.Uncommon, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new EnergyVar(1)
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await PowerCmd.Apply<UnadornedBeautyPower>(Owner.Creature, DynamicVars.Energy.IntValue, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.Energy.UpgradeValueBy(1);
  }
}
