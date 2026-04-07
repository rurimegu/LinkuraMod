using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
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

  public override Task AfterApplied(Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    TrackSubscription(Events.IncreaseMaxHearts.SubscribeEarly(OnIncreaseMaxHeartsEarly));
    return base.AfterApplied(applier, cardSource);
  }

  public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player) {
    await base.AfterPlayerTurnStart(choiceContext, player);
    if (player != Owner.Player) return;
    Flash();
    await PlayerCmd.GainEnergy(Amount, Owner.Player);
  }

  private Task OnIncreaseMaxHeartsEarly(Events.IncreaseMaxHeartsEvent ev) {
    if (ev.Player.Creature == Owner) {
      ev.Cancel();
    }
    return Task.CompletedTask;
  }
}
