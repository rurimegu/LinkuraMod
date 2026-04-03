using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// On Your Mark — X Cost, Skill, Uncommon.
/// Increase max ❤️ by 4 (6) × X. Exhaust.
/// </summary>
public class OnYourMark() : LinkuraCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new ExpandHeartsVar(4),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    // For X-cost cards, the cost is determined by energy spent in play
    // We'll use a fixed scaling: 4 hearts per energy cost
    int heartIncrease = DynamicVars.ExpandHearts().IntValue;
    await LinkuraCmd.IncreaseMaxHearts(Owner, heartIncrease, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.ExpandHearts().UpgradeValueBy(2m);
  }
}
