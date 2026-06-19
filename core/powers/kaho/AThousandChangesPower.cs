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
/// This turn, your over-capped ❤️ is converted to equal amount of ❤️ max HP.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Rare.Skill.AThousandChanges"/>.
/// </summary>
public class AThousandChangesPower() : KahoPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;

  public override Task AfterApplied(Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    TrackSubscription(Events.Burst.SubscribeLate(OnBurstLate));
    return base.AfterApplied(applier, cardSource);
  }

  private async Task OnBurstLate(Events.BurstEvent ev) {
    if (ev.Player.Creature != Owner || ev.RequestedAmount <= ev.ActualAmount) return;
    int overflow = ev.RequestedAmount - ev.ActualAmount;
    if (overflow > 0) {
      Flash();
      await HeartsState.AddMaxHearts(ev.Player, ev.Context, overflow, ev.Source);
    }
  }

  public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants) {
    await base.AfterSideTurnEnd(choiceContext, side, participants);
    if (side == Owner.Side) {
      await PowerCmd.Remove(this);
    }
  }
}
