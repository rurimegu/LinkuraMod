using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers;

/// <summary>
/// Whenever you trigger a Backstage effect, trigger Auto Burst once.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Rare.Power.FlowerKnot"/>.
/// </summary>
public class FlowerKnotPower : LinkuraPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  public override Task AfterApplied(Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    TrackSubscription(Events.TriggerBackstage.SubscribeLate(OnTriggerBackstageLate));
    return base.AfterApplied(applier, cardSource);
  }

  private async Task OnTriggerBackstageLate(Events.TriggerBackstageEvent ev) {
    if (ev.Player.Creature != Owner) return;
    await CardPileCmd.Draw(ev.Context, Amount, Owner.Player);
  }
}
