using System.Collections.Generic;
using System.Threading.Tasks;
using RuriMegu.Core.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace RuriMegu.Core.Cards.Kaho.Basic.Attack;

/// <summary>
/// Strike card for Hinoshita Kaho.
/// Basic attack: Deal 6 damage. Upgrade: Deal 9 damage.
/// </summary>
public class KahoStrike() : KahoCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) {
  protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(6, ValueProp.Move),
  ];

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play) {
    await CommonActions.CardAttack(this, play.Target).Execute(choiceContext);
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(3m);
  }
}
