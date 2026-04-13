using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers.Kaho;

/// <summary>
/// Base class for May Dreams Bloom powers.
/// </summary>
public abstract class MayDreamsBloomPowerBase : KahoPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  protected abstract int Threshold { get; }

  private const string TRACKER_VAR = "MAY_DREAMS_BLOOM_TRACKER";

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DynamicVar(TRACKER_VAR, 0),
  ];

  public override Task AfterApplied(Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    TrackSubscription(Events.Burst.SubscribeLate(OnBurstLate));
    DynamicVars[TRACKER_VAR].BaseValue = 0;
    return base.AfterApplied(applier, cardSource);
  }

  private async Task OnBurstLate(Events.BurstEvent ev) {
    if (ev.Player.Creature != Owner || ev.isAutoBurst) return;
    int overflow = ev.RequestedAmount - ev.ActualAmount;
    if (overflow <= 0) return;

    int accumulatedOverflow = (int)DynamicVars[TRACKER_VAR].BaseValue;
    accumulatedOverflow += overflow;
    while (accumulatedOverflow >= Threshold) {
      accumulatedOverflow -= Threshold;
      Flash();
      await LinkuraCmd.GainAutoBurst(Owner, Amount, Owner, null);
    }
    DynamicVars[TRACKER_VAR].BaseValue = accumulatedOverflow;
  }
}

/// <summary>
/// For every 16 ❤️ overflowed, gain 1 stack of Auto Burst.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Rare.Power.MayDreamsBloom"/> (base version).
/// </summary>
public class MayDreamsBloomPower : MayDreamsBloomPowerBase {
  protected override int Threshold => 16;
}

/// <summary>
/// For every 12 ❤️ overflowed, gain 1 stack of Auto Burst.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Rare.Power.MayDreamsBloom"/> (upgraded version).
/// </summary>
public class MayDreamsBloomUpgradedPower : MayDreamsBloomPowerBase {
  protected override int Threshold => 12;
}
