using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers;

/// <summary>
/// Whenever you Auto Burst, gain Block equal to the non-overflowing burst amount.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Uncommon.Power.SugarMelt"/>.
/// </summary>
public class SugarMeltPower : LinkuraPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  public override Task AfterApplied(Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    TrackSubscription(Events.AutoBurst.SubscribeLate(OnAutoBurstLate));
    return base.AfterApplied(applier, cardSource);
  }

  private async Task OnAutoBurstLate(Events.AutoBurstEvent ev) {
    if (ev.Player.Creature != Owner) return;
    int actual = ev.BurstEvent?.ActualAmount ?? 0;
    if (actual <= 0) return;
    await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
  }
}
