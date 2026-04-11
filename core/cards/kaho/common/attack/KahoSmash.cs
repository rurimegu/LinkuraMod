using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace RuriMegu.Core.Cards.Kaho.Common.Attack;

/// <summary>
/// Kaho Smash — Cost 1, Attack, Common.
/// Deal 8 (11) damage. Collect.
/// </summary>
public class KahoSmash() : KahoCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [LinkuraKeywords.Collect];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(8, ValueProp.Move),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play.Target).Execute(ctx);
    await LinkuraCardActions.CollectHearts(this, ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(3m);
  }
}
