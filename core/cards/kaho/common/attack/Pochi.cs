using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Attack;

/// <summary>
/// Pochi! — Cost 1, Attack, Common.
/// Deal damage equal to your current ♥. Collect. Ethereal. (Remove Ethereal on upgrade.)
/// </summary>
public class Pochi() : KahoCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
  protected override IEnumerable<string> RegisteredKeywordIds => [LinkuraKeywords.Collect];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int hearts = HeartsState.GetHearts(Owner);
    await DamageCmd.Attack(hearts)
      .FromCard(this)
      .Targeting(play.Target)
      .Execute(ctx);
    await LinkuraCardActions.CollectHearts(this, ctx);
  }

  protected override void OnUpgrade() {
    RemoveKeyword(CardKeyword.Ethereal);
  }
}
