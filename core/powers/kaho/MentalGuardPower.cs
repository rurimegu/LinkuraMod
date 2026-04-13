using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers.Kaho;

/// <summary>
/// This turn, Burst grant Block instead of hearts.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Uncommon.Skill.MentalGuard"/>.
/// </summary>
public class MentalGuardPower : KahoPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;

  public override Task AfterApplied(Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    TrackSubscription(Events.Burst.SubscribeEarly(OnBurst));
    return base.AfterApplied(applier, cardSource);
  }

  private async Task OnBurst(Events.BurstEvent ev) {
    if (ev.Player.Creature != Owner) return;
    int amount = ev.RequestedAmount;
    ev.Cancel();
    Flash();
    await CreatureCmd.GainBlock(Owner, amount, ValueProp.Unpowered, null);
  }

  public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side) {
    await base.AfterTurnEnd(choiceContext, side);
    if (side == Owner.Side) {
      await PowerCmd.Remove(this);
    }
  }
}
