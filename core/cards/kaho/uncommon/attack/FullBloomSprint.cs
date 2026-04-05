using System.Collections.Generic;
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
/// Full Bloom Sprint — Cost 2 (1), Attack, Uncommon.
/// Deal 12 (16) damage. If current ❤️ is at max, Collect and gain 2 energy.
/// </summary>
public class FullBloomSprint() : LinkuraCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {

  public override IEnumerable<CardKeyword> CanonicalKeywords => [LinkuraKeywords.Collect];
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(12, ValueProp.Move),
    new EnergyVar(2),
  ];

  protected override bool ShouldGlowGoldInternal => HeartsState.ReachedMaxHearts(Owner);

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play.Target).Execute(ctx);

    if (!ShouldGlowGoldInternal) return;

    await LinkuraCardActions.CollectHearts(this, ctx);
    await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
  }

  protected override void OnUpgrade() {
    EnergyCost.UpgradeBy(-1);
    DynamicVars.Damage.UpgradeValueBy(4m);
  }
}
