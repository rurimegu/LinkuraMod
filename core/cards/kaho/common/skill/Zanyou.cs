using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Zanyou (残阳) — Cost 1, Skill, Common.
/// Increase max ♥ by 5 (8). Ethereal (upgrade removes Ethereal). Exhaust.
/// </summary>
public class Zanyou() : LinkuraCard(1, CardType.Skill, CardRarity.Common, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [
    CardKeyword.Ethereal,
    CardKeyword.Exhaust,
  ];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new ExpandHeartsVar(5),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.IncreaseMaxHearts(this, ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.ExpandHearts().UpgradeValueBy(3m);
    RemoveKeyword(CardKeyword.Ethereal);
  }
}
