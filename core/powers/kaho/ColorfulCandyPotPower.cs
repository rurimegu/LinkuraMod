using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers.Kaho;

/// <summary>
/// This turn, whenever you Burst, Collect.
/// Applied by <see cref="RuriMegu.Core.Potions.Kaho.Rare.ColorfulCandyPot"/>.
/// </summary>
public class ColorfulCandyPotPower : KahoPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;

  public override async Task AfterApplied(Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    TrackSubscription(Events.Burst.SubscribeLate(OnBurstLate));
    await base.AfterApplied(applier, cardSource);
  }

  private async Task OnBurstLate(Events.BurstEvent ev) {
    if (ev.Player.Creature != Owner) return;
    Flash();
    await LinkuraCmd.CollectHearts(ev.Player, ev.Context, null);
  }

  public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants) {
    await base.AfterSideTurnEnd(choiceContext, side, participants);
    if (side == Owner.Side) {
      await PowerCmd.Remove(this);
    }
  }
}
