using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Genyou Yakou (眩曜夜行) — Cost 0, Skill, Common.
/// If you have 8 or more ♥, draw 2 (3) cards. Collect. Ethereal. (Remove Ethereal on upgrade.)
/// </summary>
public class GenyouYakou() : LinkuraCard(0, CardType.Skill, CardRarity.Common, TargetType.None) {
  private const int HEARTS_THRESHOLD = 8;

  public override IEnumerable<CardKeyword> CanonicalKeywords => [
    LinkuraKeywords.Collect,
    CardKeyword.Ethereal,
  ];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CardsVar(2),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    if (HeartsState.GetHearts(Owner) >= HEARTS_THRESHOLD) {
      await CommonActions.Draw(this, ctx);
    }
    await LinkuraCardActions.CollectHearts(this, ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.Cards.UpgradeValueBy(1m);
    RemoveKeyword(CardKeyword.Ethereal);
  }
}
