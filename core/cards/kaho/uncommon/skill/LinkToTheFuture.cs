using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Link to the FUTURE — Cost 1, Skill, Uncommon.
/// Trigger 2 (3) [gold]Auto Burst[/gold]. Next turn gain 1 (2) energy.
/// </summary>
public class LinkToTheFuture() : LinkuraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  private const int BASE_BURST = 1;
  private const int BASE_ENERGY_NEXT_TURN = 1;
  private const string BURSTS_VAR = "RURIMEGU-LINK_BURSTS";
  private const string ENERGY_VAR = "RURIMEGU-LINK_ENERGY";

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DynamicVar(BURSTS_VAR, 2),
    new DynamicVar(ENERGY_VAR, BASE_ENERGY_NEXT_TURN),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int bursts = DynamicVars[BURSTS_VAR].IntValue;
    for (int i = 0; i < bursts; i++) {
      await LinkuraCmd.BurstHearts(Owner, BASE_BURST, this);
    }

    // Note: Granting energy next turn would require a custom power implementation
  }

  protected override void OnUpgrade() {
    DynamicVars[BURSTS_VAR].UpgradeValueBy(1m);
    DynamicVars[ENERGY_VAR].UpgradeValueBy(1m);
  }
}
