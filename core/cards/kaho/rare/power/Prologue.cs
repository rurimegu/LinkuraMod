using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Powers.Kaho;

namespace RuriMegu.Core.Cards.Kaho.Rare.Power;

/// <summary>
/// Prologue (搴忕珷) 鈥?Cost 2, Power, Rare, (Innate.)
/// Whenever Max 鉂わ笍 changes, the next card played this turn costs 1 Energy less.
/// </summary>
public class Prologue() : KahoCard(2, CardType.Power, CardRarity.Rare, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new EnergyVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await PowerCmd.Apply<ProloguePower>(Owner.Creature, DynamicVars.Energy.IntValue, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    AddKeyword(CardKeyword.Innate);
  }
}
