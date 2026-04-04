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
/// A Thousand Changes — Cost 0, Skill, Rare.
/// Overflow ❤️ -> Max ❤️ for this turn.
/// Exhaust. (Upgraded: Remove)
/// </summary>
public class AThousandChanges() : LinkuraCard(0, CardType.Skill, CardRarity.Rare, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [
    CardKeyword.Exhaust,
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await PowerCmd.Apply<AThousandChangesPower>(Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    RemoveKeyword(CardKeyword.Exhaust);
  }
}
