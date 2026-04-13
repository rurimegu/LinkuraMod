using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Cards.Kaho.Ancient;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Basic.Skill;

/// <summary>
/// Wide Heart (广域之心) — Cost 0, Skill, Basic.
/// Increase Max Hearts by 2 (4). Draw 1 card.
/// </summary>
public class WideHeart() : KahoCard(0, CardType.Skill, CardRarity.Basic, TargetType.None), ITranscendenceCard {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new ExpandHeartsVar(2),
    new CardsVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play) {
    await LinkuraCardActions.IncreaseMaxHearts(this, choiceContext);
    await CommonActions.Draw(this, choiceContext);
  }

  protected override void OnUpgrade() {
    DynamicVars.ExpandHearts().UpgradeValueBy(2m);
  }

  public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<BloomingHeart>();
}
