using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Agile Steps (灵动舞步) — Cost 1, Skill, Common.
/// Burst 5 (8). Your next Attack card costs 1 less energy.
/// </summary>
public class AgileSteps() : LinkuraCard(1, CardType.Skill, CardRarity.Common, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BurstHeartsVar(5),
    new EnergyVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.BurstHearts(this, ctx);
    await PowerCmd.Apply<AttackCostReductionPower>(Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.BurstHearts().UpgradeValueBy(3m);
  }
}
