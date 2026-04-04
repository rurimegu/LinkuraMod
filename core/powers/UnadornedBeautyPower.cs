using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers;

/// <summary>
/// Start of turn, gain 1 Energy. Cannot increase max ❤️.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Uncommon.Power.UnadornedBeauty"/>.
/// </summary>
public class UnadornedBeautyPower : LinkuraPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  private Subscription _sub;

  public override Task AfterApplied(Creature applier, CardModel cardSource) {
    _sub?.Dispose();
    _sub = Events.IncreaseMaxHearts.SubscribeVeryEarly(OnIncreaseMaxHeartsEarly);
    return base.AfterApplied(applier, cardSource);
  }

  public override Task AfterRemoved(Creature oldOwner) {
    _sub?.Dispose();
    _sub = null;
    return base.AfterRemoved(oldOwner);
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    _sub?.Dispose();
    _sub = null;
    return base.AfterCombatEnd(room);
  }

  public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState) {
    await base.BeforeSideTurnStart(choiceContext, side, combatState);
    if (side != Owner.Side) return;
    await PlayerCmd.GainEnergy((int)Amount, Owner.Player);
  }

  private Task OnIncreaseMaxHeartsEarly(Events.IncreaseMaxHeartsEvent ev) {
    if (ev.Player.Creature == Owner) {
      ev.Cancel();
    }
    return Task.CompletedTask;
  }
}
