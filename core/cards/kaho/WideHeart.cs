using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho;

public class WideHeart() : LinkuraCard(1, CardType.Skill, CardRarity.Basic, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new ExpandHeartsVar(2),
    new CardsVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play) {
    await LinkuraCardActions.IncreaseMaxHearts(this);
    await CommonActions.Draw(this, choiceContext);
  }

  protected override void OnUpgrade() {
    DynamicVars.ExpandHearts().UpgradeValueBy(2m);
  }
}
