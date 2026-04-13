using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Powers.Kaho;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Power;

/// <summary>
/// Training Camp (合宿) — Cost 2, Power, Uncommon.
/// The first 2 (3) Backstage cards you play each turn cost 1 less.
/// </summary>
public class TrainingCamp() : KahoCard(2, CardType.Power, CardRarity.Uncommon, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new RepeatVar(2),
    new EnergyVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await Owner.PlayCastAnim();
    await PowerCmd.Apply<TrainingCampPower>(Owner.Creature, DynamicVars.Repeat.IntValue, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.Repeat.UpgradeValueBy(1m);
  }
}
