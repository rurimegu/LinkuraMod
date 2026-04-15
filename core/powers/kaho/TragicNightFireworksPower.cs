using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers.Kaho;

/// <summary>
/// Base class for Tragic Night Fireworks powers.
/// Locks max ❤️ at 99.
/// </summary>
public abstract class TragicNightFireworksPowerBase : KahoPower {
  protected const int FIXED_MAX_HEARTS = 99;

  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;

  public override async Task AfterApplied(Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    await HeartsState.SetMaxHearts(Owner.Player, Events.BLOCKING_CONTEXT, FIXED_MAX_HEARTS, cardSource);
    TrackSubscription(Events.MaxHeartsChanged.SubscribeEarly(OnMaxHeartsChangedEarly));
    await base.AfterApplied(applier, cardSource);
  }

  protected virtual Task OnMaxHeartsChangedEarly(Events.MaxHeartsChangedEvent ev) {
    if (ev.Player != Owner.Player) return Task.CompletedTask;
    if (ev.NewMaxHearts != FIXED_MAX_HEARTS) {
      ev.Cancel();
      Flash();
    }
    return Task.CompletedTask;
  }
}

/// <summary>
/// Base version: Locks max ❤️ at 99.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Ancient.TragicNightFireworks"/>.
/// </summary>
public class TragicNightFireworksPower : TragicNightFireworksPowerBase {
  public override async Task AfterApplied(Creature applier, CardModel cardSource) {
    await base.AfterApplied(applier, cardSource);
    // If upgraded power is already present, remove itself.
    if (Owner.Powers.OfType<TragicNightFireworksUpgradedPower>().Any()) {
      await PowerCmd.Remove(this);
    }
  }
}

/// <summary>
/// Upgraded version: Locks max ❤️ at 99 and redirects increase to Max HP.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Ancient.TragicNightFireworks"/>.
/// </summary>
public class TragicNightFireworksUpgradedPower : TragicNightFireworksPowerBase {
  public override async Task AfterApplied(Creature applier, CardModel cardSource) {
    await base.AfterApplied(applier, cardSource);
    TrackSubscription(Events.IncreaseMaxHearts.SubscribeEarly(OnIncreaseMaxHeartsEarly));
    // Remove base power if present.
    var basePower = Owner.Powers.OfType<TragicNightFireworksPower>().FirstOrDefault();
    if (basePower != null) {
      await PowerCmd.Remove(basePower);
    }
  }

  private Task OnIncreaseMaxHeartsEarly(Events.IncreaseMaxHeartsEvent ev) {
    if (ev.Player != Owner.Player) return Task.CompletedTask;
    ev.GainMaxHpInstead = true;
    return Task.CompletedTask;
  }
}
