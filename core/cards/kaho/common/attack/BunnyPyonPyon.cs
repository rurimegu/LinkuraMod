using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Attack;

/// <summary>
/// Bunny Pyon Pyon — Cost 1, Attack, Common.
/// Deal 7 (10) damage. Burst equal to the damage dealt.
/// </summary>
public class BunnyPyonPyon() : KahoCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(7, ValueProp.Move),
  ];

  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [BurstHeartsVar.HoverTip()];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    var results = await CreatureCmd.Damage(
      ctx,
      play.Target,
      DynamicVars.Damage,
      Owner.Creature,
      this);

    int totalDamage = results.Sum(r => r.TotalDamage + r.OverkillDamage);
    if (totalDamage > 0) {
      await LinkuraCmd.BurstHearts(Owner, ctx, totalDamage, this);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(3m);
  }
}
