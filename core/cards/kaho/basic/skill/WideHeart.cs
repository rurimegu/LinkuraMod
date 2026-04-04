using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Basic.Skill;

/// <summary>
/// Wide Heart (广域之心) — Cost 1, Skill, Basic.
/// Increase Max Hearts by 2 (3). Draw 1 (2) cards.
/// </summary>
public class WideHeart() : LinkuraCard(1, CardType.Skill, CardRarity.Basic, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new ExpandHeartsVar(2),
    new CardsVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play) {
    await LinkuraCardActions.IncreaseMaxHearts(this, choiceContext);
    await CommonActions.Draw(this, choiceContext);
  }

  protected override void OnUpgrade() {
    DynamicVars.ExpandHearts().UpgradeValueBy(1m);
    DynamicVars.Cards.UpgradeValueBy(1m);
  }
}
