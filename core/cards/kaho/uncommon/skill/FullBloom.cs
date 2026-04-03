using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Full Bloom! (盛开！) — Cost 0, Skill, Uncommon.
/// If your ❤️ is at max, gain {Energy:energyIcons()} and draw {Cards:diff()} card(s).
/// </summary>
public class FullBloom() : LinkuraCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new EnergyVar(1),
    new CardsVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int currentHearts = HeartsState.GetHearts(Owner);
    int maxHearts = HeartsState.GetMaxHearts(Owner);

    if (currentHearts >= maxHearts) {
      await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
      await CommonActions.Draw(this, ctx);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.Cards.UpgradeValueBy(1m);
  }
}
