using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Attack;

/// <summary>
/// Cerise Bouquet — Cost 2, Attack, Common.
/// Deal 6 (9) damage to ALL enemies 3 times.
/// </summary>
public class CeriseBouquet() : KahoCard(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(6, ValueProp.Move),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play, hitCount: 3).Execute(ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(3m);
  }
}
