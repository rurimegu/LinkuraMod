using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using RuriMegu.Core.Powers.Kaho;

namespace RuriMegu.Core.Cards.Kaho.Rare.Skill;

/// <summary>
/// A Thousand Changes — Cost 0, Skill, Rare.
/// Overflow ❤️ -> Max ❤️ for this turn.
/// Exhaust. (Upgraded: Remove)
/// </summary>
public class AThousandChanges() : KahoCard(0, CardType.Skill, CardRarity.Rare, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [
    CardKeyword.Exhaust,
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await PowerCmd.Apply<AThousandChangesPower>(ctx, Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    RemoveKeyword(CardKeyword.Exhaust);
  }
}
