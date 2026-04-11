using System;
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

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Attack;

/// <summary>
/// Heart Strike — Cost 2, Attack, Uncommon.
/// Deal 11 (15) damage. Increase max ❤️ by unblocked damage dealt.
/// </summary>
public class HeartStrike() : KahoCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {
  protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
  public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Append(CardKeyword.Exhaust);

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(11, ValueProp.Move),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    if (play.Target == null) return;

    var attackCmd = await CommonActions.CardAttack(this, play.Target).Execute(ctx);
    int totalDamage = attackCmd.Results.Aggregate(
      0,
      (totalDamage, result) => totalDamage + result.UnblockedDamage + result.OverkillDamage);

    if (totalDamage <= 0) return;
    await HeartsState.AddMaxHearts(Owner, ctx, totalDamage, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(4m);
  }
}
