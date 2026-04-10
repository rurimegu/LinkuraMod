using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Attack;

/// <summary>
/// Step Up! — Cost 1, Attack, Common.
/// Deal 4 (6) damage twice. Increase max ♥ by 2 (3).
/// </summary>
public class StepUp() : LinkuraCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(4, ValueProp.Move),
    new RepeatVar(2),
    new ExpandHeartsVar(2),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play.Target, DynamicVars.Repeat.IntValue).Execute(ctx);
    await LinkuraCardActions.IncreaseMaxHearts(this, ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(2m);
    DynamicVars.ExpandHearts().UpgradeValueBy(1m);
  }
}
