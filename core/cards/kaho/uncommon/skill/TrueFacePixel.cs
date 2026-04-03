using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// True Face Pixel (素颜的像素) — Cost 1, Skill, Uncommon.
/// This turn, your next Burst Heart card deals damage equal to Burst count to ALL enemies.
/// </summary>
public class TrueFacePixel() : LinkuraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    // This is a placeholder - the actual effect would require a custom power  
    // to track and modify the next burst effect
    await Task.CompletedTask;
  }

  protected override void OnUpgrade() {
    RemoveKeyword(CardKeyword.Ethereal);
  }
}
