using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Attack;

/// <summary>
/// Budding Premonition — Cost 2, Attack, Common, Exhaust.
/// Increase max ♥ by 8 (11). Deal damage to ALL enemies equal to current max ♥.
/// </summary>
public class BuddingPremonition() : KahoCard(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new ExpandHeartsVar(8),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.IncreaseMaxHearts(this, ctx);
    int maxHearts = HeartsState.GetMaxHearts(Owner);
    await CommonActions.CardAttack(this, play.Target, damage: (decimal)maxHearts)
      .Execute(ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.ExpandHearts().UpgradeValueBy(3m);
  }
}
