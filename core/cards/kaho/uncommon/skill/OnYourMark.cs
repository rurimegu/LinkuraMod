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
/// Increase max ❤️ by 4 (6) X. Exhaust.
/// </summary>
public class OnYourMark() : LinkuraCard(-1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new ExpandHeartsVar(4),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int heartIncrease = DynamicVars.ExpandHearts().IntValue * play.Resources.EnergySpent;
    if (heartIncrease > 0) {
      await LinkuraCmd.IncreaseMaxHearts(Owner, ctx, heartIncrease, this);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.ExpandHearts().UpgradeValueBy(2m);
  }
}
