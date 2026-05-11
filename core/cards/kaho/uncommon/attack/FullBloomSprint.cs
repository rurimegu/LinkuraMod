using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Attack;

/// <summary>
/// Full Bloom Sprint — Cost 2 (1), Attack, Uncommon.
/// Deal 16 damage. If your ❤️ count is less than half your max ❤️, gain 2 energy.
/// </summary>
public class FullBloomSprint() : KahoCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(16, ValueProp.Move),
    new EnergyVar(2),
  ];

  protected override bool ShouldGlowGoldInternal => !HeartsState.ReachedHalfHearts(Owner);

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play.Target).Execute(ctx);

    if (!ShouldGlowGoldInternal) return;

    await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
  }

  protected override void OnUpgrade() {
    EnergyCost.UpgradeBy(-1);
  }
}
