using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Attack;

/// <summary>
/// Budding Premonition — Cost 2, Attack, Common, Exhaust.
/// Increase max ♥ by 5 (8). Deal damage equal to current max ♥.
/// </summary>
public class BuddingPremonition() : LinkuraCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new ExpandHeartsVar(5),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.IncreaseMaxHearts(this, ctx);
    int maxHearts = HeartsState.GetMaxHearts(Owner);
    await DamageCmd.Attack(maxHearts)
      .FromCard(this)
      .Targeting(play.Target)
      .Execute(ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.ExpandHearts().UpgradeValueBy(3m);
  }
}
