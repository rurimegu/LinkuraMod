using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Link to the FUTURE — Cost 1, Skill, Uncommon.
/// Trigger 2 (3) [gold]Auto Burst[/gold]. Next turn gain 1 (2) energy.
/// </summary>
public class LinkToTheFuture() : LinkuraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  private const int BASE_ENERGY_NEXT_TURN = 1;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new TriggerAutoBurstVar(2),
    new EnergyVar(BASE_ENERGY_NEXT_TURN),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.AutoBurst(this, ctx);

    int energyGain = DynamicVars.Energy.IntValue;
    await PowerCmd.Apply<EnergyNextTurnPower>(Owner.Creature, (decimal)energyGain, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.TriggerAutoBurst().UpgradeValueBy(1m);
    DynamicVars.Energy.UpgradeValueBy(1m);
  }
}
