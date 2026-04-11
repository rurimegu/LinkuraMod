using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Powers.Kaho;

namespace RuriMegu.Core.Cards.Kaho.Common.Attack;

/// <summary>
/// 37.5°C's Fantasy — Cost 3, Attack, Common.
/// Deal 37 damage. Your next Power card costs 2 less (upgraded: 3 less).
/// Uses <see cref="PowerCostReductionPower"/> which expires after the first Power card is played.
/// </summary>
public class Fantasy375() : KahoCard(3, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(37, ValueProp.Move),
    new EnergyVar(2)
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play.Target).Execute(ctx);
    // Apply a power that reduces the next Power card's cost.
    // The power expires after the first Power card is played.
    int reduction = DynamicVars.Energy.IntValue;
    await PowerCmd.Apply<PowerCostReductionPower>(Owner.Creature, reduction, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.Energy.UpgradeValueBy(1m);
  }
}
