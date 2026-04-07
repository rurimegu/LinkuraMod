using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers;

/// <summary>
/// Whenever you trigger a Backstage effect, it triggers an additional time for each stack.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Uncommon.Power.HolidayHoliday"/>.
/// </summary>
public class HolidayHolidayPower : LinkuraPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  public override Task AfterApplied(Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    TrackSubscription(Events.TriggerBackstage.SubscribeEarly(OnTriggerBackstageEarly));
    return base.AfterApplied(applier, cardSource);
  }

  private Task OnTriggerBackstageEarly(Events.TriggerBackstageEvent ev) {
    if (ev.Player.Creature == Owner) {
      ev.RepeatCount += Amount;
      Flash();
    }
    return Task.CompletedTask;
  }
}
