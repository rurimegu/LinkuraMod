using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;
using STS2RitsuLib.Keywords;

namespace RuriMegu.Core.Cards.Kaho.Common.Attack;

/// <summary>
/// Kaho Smash — Cost 1, Attack, Common.
/// Deal 8 (11) damage. Collect.
/// </summary>
public class KahoSmash() : KahoCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Append(LinkuraKeywords.Collect.GetModCardKeyword());

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(8, ValueProp.Move),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play.Target).Execute(ctx);
    await LinkuraCardActions.CollectHearts(this, ctx, target: play.Target);
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(4m);
  }
}
