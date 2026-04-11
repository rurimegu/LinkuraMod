using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Rare.Skill;

/// <summary>
/// Excessive Training 鈥?Cost 0, Skill, Rare.
/// Reduce 4 (3) Max 鉂わ笍 up to 3 (4) times. Gain [E] for each reduction.
/// </summary>
public class ExcessiveTraining() : KahoCard(0, CardType.Skill, CardRarity.Rare, TargetType.None) {
  private const string MAX_TIMES_KEY = "RURIMEGU-MAX_TIMES";

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new MaxHeartsThresholdVar(4m),
    new DynamicVar(MAX_TIMES_KEY, 3m),
    new EnergyVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int reduction = DynamicVars.MaxHeartThreshold().IntValue;
    int maxTimes = DynamicVars[MAX_TIMES_KEY].IntValue;

    for (int i = 0; i < maxTimes; i++) {
      if (HeartsState.GetMaxHearts(Owner) <= reduction) break;
      var ev = await HeartsState.AddMaxHearts(Owner, ctx, -reduction, this);
      if (ev.IsNullOrCancelled()) break;
      await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.MaxHeartThreshold().UpgradeValueBy(-1m);
    DynamicVars[MAX_TIMES_KEY].UpgradeValueBy(1m);
  }
}
