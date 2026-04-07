using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers;

/// <summary>
/// This turn, Max ❤️ increase -> Auto Burst + Collect.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Rare.Skill.EnduringTradition"/>.
/// </summary>
public class EnduringTraditionPower() : LinkuraPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;

  public override Task AfterApplied(Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    TrackSubscription(Events.IncreaseMaxHearts.SubscribeVeryEarly(OnIncreaseMaxHeartsEarly));
    return base.AfterApplied(applier, cardSource);
  }

  private async Task OnIncreaseMaxHeartsEarly(Events.IncreaseMaxHeartsEvent ev) {
    if (ev.Player.Creature != Owner || ev.RequestedAmount <= 0) return;

    int amount = ev.RequestedAmount;
    ev.Cancel();
    Flash();

    for (int i = 0; i < amount; i++) {
      await LinkuraCmd.TriggerAutoBurst(ev.Player, ev.Context, ev.Source);
      await LinkuraCmd.CollectHearts(ev.Player, ev.Context, ev.Source);
    }
  }

  public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side) {
    await base.AfterTurnEnd(choiceContext, side);
    if (side == Owner.Side) {
      await PowerCmd.Remove(this);
    }
  }
}
