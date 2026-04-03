using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Love it! — Cost 1, Skill, Common.
/// Burst 8 (11). Draw 1 (2) cards.
/// </summary>
public class LoveIt() : LinkuraCard(1, CardType.Skill, CardRarity.Common, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BurstHeartsVar(8),
    new CardsVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.BurstHearts(this, ctx);
    await CommonActions.Draw(this, ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.BurstHearts().UpgradeValueBy(3m);
    DynamicVars.Cards.UpgradeValueBy(1m);
  }
}
