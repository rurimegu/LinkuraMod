using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers;

/// <summary>
/// Whenever you Collect Hearts, deal damage to all enemies.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Uncommon.Power.SpecialAppeal"/>.
/// </summary>
public class SpecialAppealPower : LinkuraPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;

  private Subscription _sub;

  public override Task AfterApplied(Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    TrackSubscription(Events.Collect.SubscribeEarly(OnCollectEarly));
    return base.AfterApplied(applier, cardSource);
  }

  private Task OnCollectEarly(Events.CollectEvent ev) {
    if (ev.Player.Creature == Owner) {
      ev.DamageAllEnemies = true;
      Flash();
    }
    return Task.CompletedTask;
  }
}
