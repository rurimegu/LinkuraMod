using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers.Kaho;

/// <summary>
/// Permanently locks max ❤️ at 99 by cancelling any change that would differ from 99.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Ancient.TragicNightFireworks"/>.
/// </summary>
public class TragicNightFireworksPower : KahoPower {
  private const int FIXED_MAX_HEARTS = 99;

  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;

  public override async Task AfterApplied(MegaCrit.Sts2.Core.Entities.Creatures.Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    await HeartsState.SetMaxHearts(Owner.Player, Events.BLOCKING_CONTEXT, FIXED_MAX_HEARTS, cardSource);
    TrackSubscription(Events.MaxHeartsChanged.SubscribeEarly(OnMaxHeartsChangedEarly));
    await base.AfterApplied(applier, cardSource);
  }

  private Task OnMaxHeartsChangedEarly(Events.MaxHeartsChangedEvent ev) {
    if (ev.Player != Owner.Player) return Task.CompletedTask;
    if (ev.NewMaxHearts != FIXED_MAX_HEARTS) {
      ev.Cancel();
      Flash();
    }
    return Task.CompletedTask;
  }
}
