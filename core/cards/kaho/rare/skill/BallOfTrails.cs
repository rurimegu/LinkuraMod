using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Rare.Skill;

/// <summary>
/// Ball of Trails — Cost 1, Skill, Rare.
/// Burst 10 (15).
/// If drawn at the start of your turn, gain 2 (3) energy, discard your hand and draw that many cards.
/// </summary>
public class BallOfTrails() : LinkuraCard(1, CardType.Skill, CardRarity.Rare, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BurstHeartsVar(10),
    new EnergyVar(2),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.BurstHearts(this, ctx);
  }

  public override async Task AfterCardDrawn(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw) {
    if (card != this || !fromHandDraw) return;
    await Cmd.Wait(0.25f);
    await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    await LinkuraCardActions.DiscardAndDraw(this, ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.BurstHearts().UpgradeValueBy(5m);
    DynamicVars.Energy.UpgradeValueBy(1m);
  }
}
