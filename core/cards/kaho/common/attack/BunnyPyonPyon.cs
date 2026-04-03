using System.Collections.Generic;
using System.Linq;
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
/// Bunny Pyon Pyon — Cost 1, Attack, Common.
/// Deal 6 (9) damage. Burst equal to the damage dealt.
/// </summary>
public class BunnyPyonPyon() : LinkuraCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(6, ValueProp.Move),
    new BurstHeartsVar(6),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    var results = await CreatureCmd.Damage(
      ctx,
      play.Target,
      DynamicVars.Damage,
      Owner.Creature,
      this);

    int totalDamage = results.Sum(r => r.UnblockedDamage + r.OverkillDamage);
    if (totalDamage > 0) {
      await LinkuraCmd.BurstHearts(Owner, ctx, totalDamage, this);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(3m);
    DynamicVars.BurstHearts().UpgradeValueBy(3m);
  }
}
