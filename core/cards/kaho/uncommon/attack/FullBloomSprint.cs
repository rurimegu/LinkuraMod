using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Attack;

/// <summary>
/// Full Bloom Sprint — Cost 2 (1), Attack, Uncommon.
/// Deal 8 (11) damage. If current ❤️ is at max, Collect and gain 2 energy.
/// </summary>
public class FullBloomSprint() : LinkuraCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(8, ValueProp.Move),
    new EnergyVar(2),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play.Target).Execute(ctx);

    int hearts = HeartsState.GetHearts(Owner);
    int maxHearts = HeartsState.GetMaxHearts(Owner);
    if (hearts < maxHearts) return;

    await LinkuraCardActions.CollectHearts(this, ctx);
    await PlayerCmd.GainEnergy(2, Owner);
  }

  protected override void OnUpgrade() {
    EnergyCost.UpgradeBy(-1);
    DynamicVars.Damage.UpgradeValueBy(3m);
  }
}
