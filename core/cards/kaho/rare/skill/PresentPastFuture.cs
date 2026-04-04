using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Rare.Skill;

/// <summary>
/// Present, Past, Future — Cost 1, Skill, Rare.
/// Discard hand, draw same amount.
/// Backstage: every 30 (20) Burst, trigger this effect.
/// </summary>
public class PresentPastFuture() : InHandTriggerCard(1, CardType.Skill, CardRarity.Rare, TargetType.None) {
  private const string TRACKER_VAR = "EXPECTATION_TRACKER";
  private const string THRESHOLD_VAR = "RURIMEGU-EXPECTATION_THRESHOLD";

  private Subscription _sub;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DynamicVar(THRESHOLD_VAR, 30m),
    new DynamicVar(TRACKER_VAR, 0m),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.DiscardAndDraw(this, ctx);
  }

  public override Task BeforeCombatStartLate() {
    _sub?.Dispose();
    _sub = Events.Collect.SubscribeLate(OnCollectLate);
    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    _sub?.Dispose();
    _sub = null;
    DynamicVars[TRACKER_VAR].BaseValue = 0;
    return Task.CompletedTask;
  }

  private async Task OnCollectLate(Events.CollectEvent ev) {
    if (ev.Player != Owner || ev.Amount <= 0 || !this.IsInHand()) return;

    DynamicVars[TRACKER_VAR].BaseValue += ev.Amount;
    int threshold = DynamicVars[THRESHOLD_VAR].IntValue;

    while (DynamicVars[TRACKER_VAR].IntValue >= threshold) {
      int newTracker = DynamicVars[TRACKER_VAR].IntValue - threshold;
      await TriggerWithAction(ev.Context, () => {
        DynamicVars[TRACKER_VAR].BaseValue = newTracker;
        return LinkuraCardActions.DiscardAndDraw(this, ev.Context);
      });
    }
  }

  protected override void OnUpgrade() {
    DynamicVars[THRESHOLD_VAR].UpgradeValueBy(-10m);
  }
}
