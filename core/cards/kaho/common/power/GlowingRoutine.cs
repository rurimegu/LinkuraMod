using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Powers.Kaho;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Power;

/// <summary>
/// Glowing Routine (闪耀的日常) — Cost 1, Power, Common.
/// At the start of your turn, Burst 6 (9).
/// </summary>
public class GlowingRoutine() : KahoCard(1, CardType.Power, CardRarity.Common, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BurstHeartsVar(6),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await Owner.PlayCastAnim();
    await PowerCmd.Apply<GlowingRoutinePower>(Owner.Creature, DynamicVars.BurstHearts().IntValue, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.BurstHearts().UpgradeValueBy(3m);
  }
}
