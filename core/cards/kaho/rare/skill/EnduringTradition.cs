using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Rare.Skill;

/// <summary>
/// Enduring Tradition — Cost 1 (0), Skill, Rare.
/// Apply Enduring Tradition Power.
/// </summary>
public class EnduringTradition() : LinkuraCard(1, CardType.Skill, CardRarity.Rare, TargetType.None) {
  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await PowerCmd.Apply<EnduringTraditionPower>(Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    EnergyCost.UpgradeBy(-1);
  }
}
