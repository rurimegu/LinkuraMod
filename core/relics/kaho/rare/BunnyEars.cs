using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Relics.Kaho.Rare;

/// <summary>
/// Bunny Ears — Rare relic for Hinoshita Kaho.
/// For every 10 ❤️ overflowed, deal 1 damage to all enemies.
/// Overflow accumulates across combats.
/// </summary>
public class BunnyEars : KahoRelic {
  public override RelicRarity Rarity => RelicRarity.Rare;

  private const int OVERFLOW_THRESHOLD = 10;

  private int _accumulatedOverflow;

  [SavedProperty]
  public int AccumulatedOverflow {
    get => _accumulatedOverflow;
    set {
      AssertMutable();
      _accumulatedOverflow = value;
      InvokeDisplayAmountChanged();
    }
  }

  public override bool ShowCounter => true;
  public override int DisplayAmount => AccumulatedOverflow;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(3m, ValueProp.Unpowered),
  ];

  protected override Task InitializeSubscriptions() {
    TrackSubscription(Events.Burst.SubscribeLate(OnBurstLate));
    return Task.CompletedTask;
  }

  private async Task OnBurstLate(Events.BurstEvent ev) {
    if (ev.Player != Owner) return;
    int overflow = ev.RequestedAmount - ev.ActualAmount;
    if (overflow <= 0) return;
    AccumulatedOverflow += overflow;
    while (AccumulatedOverflow >= OVERFLOW_THRESHOLD) {
      AccumulatedOverflow -= OVERFLOW_THRESHOLD;
      Flash();
      await CreatureCmd.Damage(
        ev.Context,
        Owner.Creature.CombatState.HittableEnemies,
        DynamicVars.Damage.BaseValue,
        DynamicVars.Damage.Props,
        Owner.Creature,
        null
      );
    }
  }
}
