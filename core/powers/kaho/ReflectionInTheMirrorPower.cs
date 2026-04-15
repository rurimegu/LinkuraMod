using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers.Kaho;

/// <summary>
/// Effects that increase Max ❤️ are doubled.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Rare.Power.ReflectionInTheMirror"/>.
/// </summary>
public class ReflectionInTheMirrorPower : KahoPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;

  public override Task AfterApplied(Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    TrackSubscription(Events.IncreaseMaxHearts.SubscribeLate(OnIncreaseMaxHeartsLate));
    return base.AfterApplied(applier, cardSource);
  }

  private async Task OnIncreaseMaxHeartsLate(Events.IncreaseMaxHeartsEvent ev) {
    if (ev.Player.Creature != Owner) return;
    // Prevent infinite recursion
    if (ev.ActualAmount <= 0 || ev.Source is ReflectionInTheMirrorPower) return;
    Flash();
    await LinkuraCmd.IncreaseMaxHearts(ev.Player, ev.Context, ev.ActualAmount, this);
  }
}
