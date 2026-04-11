using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;
using static RuriMegu.Core.Utils.Events;

namespace RuriMegu.Core.Powers.Kaho;

/// <summary>
/// Whenever your 鉂わ笍 hits maximum, Collect.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Uncommon.Power.AutoCollectOn"/>.
/// </summary>
public class AutoCollectOnPower : KahoPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;

  public override async Task AfterApplied(Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    TrackSubscription(Events.HeartsChanged.SubscribeLate(OnHeartsChangedLate));
    TrackSubscription(Events.MaxHeartsChanged.SubscribeLate(OnMaxHeartsChangedLate));
    await base.AfterApplied(applier, cardSource);
  }

  private Task<CollectEvent> Trigger(PlayerChoiceContext context) {
    Flash();
    return LinkuraCmd.CollectHearts(Owner.Player, context);
  }

  private async Task OnHeartsChangedLate(Events.HeartsChangedEvent ev) {
    if (ev.Player.Creature != Owner) return;
    if (ev.NewHearts < ev.MaxHearts || ev.MaxHearts <= 0) return;
    await Trigger(ev.Context);
  }

  private async Task OnMaxHeartsChangedLate(Events.MaxHeartsChangedEvent ev) {
    if (ev.Player.Creature != Owner) return;
    if (ev.NewMaxHearts <= 0 || ev.NewMaxHearts == ev.OldMaxHearts) return;
    if (ev.Hearts < ev.NewMaxHearts) return;
    await Trigger(ev.Context);
  }
}
