using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Genyou Yakou (眩曜夜行) — Cost 1 (0), Skill, Common.
/// Ethereal. If you have 10 or more ♥, draw 2 cards. Collect.
/// </summary>
public class GenyouYakou() : LinkuraCard(1, CardType.Skill, CardRarity.Common, TargetType.None) {
  private const int HEARTS_THRESHOLD = 10;

  public override IEnumerable<CardKeyword> CanonicalKeywords => [
    CardKeyword.Ethereal,
    LinkuraKeywords.Collect,
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
    EnergyCost.UpgradeBy(-1);
  }
}
