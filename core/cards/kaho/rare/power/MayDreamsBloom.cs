using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Rare.Power;

/// <summary>
/// May Dreams Bloom (愿梦想绽放一生) — Cost 2, Power, Rare.
/// For every 20 (15) ❤️ overflowed, gain 1 stack of Auto Burst.
/// </summary>
public class MayDreamsBloom() : LinkuraCard(2, CardType.Power, CardRarity.Rare, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new MaxHeartsThresholdVar(20),
    new AutoBurstVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    if (IsUpgraded) {
      await PowerCmd.Apply<MayDreamsBloomUpgradedPower>(
        Owner.Creature, DynamicVars.AutoBurst().IntValue, Owner.Creature, this);
    } else {
      await PowerCmd.Apply<MayDreamsBloomPower>(
        Owner.Creature, DynamicVars.AutoBurst().IntValue, Owner.Creature, this);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.MaxHeartThreshold().UpgradeValueBy(-5m);
  }
}
