using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Mental Guard (精神守护) — Cost 1, Skill, Uncommon.
/// This turn, Burst Hearts grant Block instead of hearts. Equal amount of Block.
/// </summary>
public class MentalGuard() : LinkuraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    // This is a placeholder - the actual effect would require a custom power
    // For now, this card simply applies Ethereal keyword
    await Task.CompletedTask;
  }

  protected override void OnUpgrade() {
    RemoveKeyword(CardKeyword.Ethereal);
  }
}
