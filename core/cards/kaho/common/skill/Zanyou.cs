using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Zanyou (残阳) — Cost 1, Skill, Common.
/// Increase max ♥ by 8 (11). Burst 8 (11). Ethereal.
/// </summary>
public class Zanyou() : KahoCard(1, CardType.Skill, CardRarity.Common, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [
    CardKeyword.Ethereal,
  ];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new ExpandHeartsVar(8),
    new BurstHeartsVar(8),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.IncreaseMaxHearts(this, ctx);
    await LinkuraCardActions.BurstHearts(this, ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.ExpandHearts().UpgradeValueBy(3m);
    DynamicVars.BurstHearts().UpgradeValueBy(3m);
  }
}
