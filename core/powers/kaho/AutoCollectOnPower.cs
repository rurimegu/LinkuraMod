using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;
using static RuriMegu.Core.Utils.Events;

namespace RuriMegu.Core.Powers.Kaho;

/// <summary>
/// Whenever your ❤️ hits maximum, Collect.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Uncommon.Power.AutoCollectOn"/>.
/// </summary>
public class AutoCollectOnPower : KahoPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;

  public override async Task AfterApplied(Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    TrackSubscription(HeartsChanged.SubscribeLate(OnHeartsChangedLate));
    TrackSubscription(MaxHeartsChanged.SubscribeLate(OnMaxHeartsChangedLate));
    if (HeartsState.ReachedMaxHearts(Owner.Player)) {
      await LinkuraCmd.CollectHearts(Owner.Player, new BlockingPlayerChoiceContext());
    }
    await base.AfterApplied(applier, cardSource);
  }

  private async Task Trigger(PlayerChoiceContext context) {
    if (CombatManager.Instance.IsOverOrEnding) return;
    Flash();
    await LinkuraCmd.CollectHearts(Owner.Player, context);
  }

  private async Task OnHeartsChangedLate(HeartsChangedEvent ev) {
    if (ev.Player.Creature != Owner) return;
    if (ev.NewHearts < ev.MaxHearts || ev.MaxHearts <= 0) return;
    await Trigger(ev.Context);
  }

  private async Task OnMaxHeartsChangedLate(MaxHeartsChangedEvent ev) {
    if (ev.Player.Creature != Owner) return;
    if (ev.NewMaxHearts <= 0 || ev.NewMaxHearts == ev.OldMaxHearts) return;
    if (ev.Hearts < ev.NewMaxHearts) return;
    await Trigger(ev.Context);
  }
}
