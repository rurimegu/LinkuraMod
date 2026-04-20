using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Choreography Memo (舞步编排) — Cost 1, Skill, Uncommon.
/// On play: Burst 3, a total of 3 (4) times.
/// Backstage: Every 4 (3) times you burst, Collect. (Current: X)
/// </summary>
public class ChoreographyMemo() : KahoInHandTriggerCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  private const string TRACKER_VAR = "CHOREOGRAPHY_TRACKER";
  private const string THRESHOLD_VAR = "CHOREOGRAPHY_THRESHOLD";

  public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Append(LinkuraKeywords.Collect);

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BurstHeartsVar(3),
    new RepeatVar(3),
    new DynamicVar(THRESHOLD_VAR, 4),
    new DynamicVar(TRACKER_VAR, 0),
  ];

  protected override Task InitializeSubscriptions() {
    TrackSubscription(Events.Burst.SubscribeLate(OnBurst));
    return Task.CompletedTask;
  }

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int times = DynamicVars.Repeat.IntValue;
    for (int i = 0; i < times; i++) {
      await LinkuraCardActions.BurstHearts(this, ctx);
    }
  }

  private async Task OnBurst(Events.BurstEvent ev) {
    if (ev.Player != Owner || ev.ActualAmount <= 0 || !CanTrigger()) return;

    DynamicVars[TRACKER_VAR].BaseValue++;

    int threshold = DynamicVars[THRESHOLD_VAR].IntValue;
    while (DynamicVars[TRACKER_VAR].IntValue >= threshold) {
      DynamicVars[TRACKER_VAR].BaseValue -= threshold;
      var triggerEv = await TriggerWithAction(ev.Context, async () => {
        await LinkuraCardActions.CollectHearts(this, ev.Context);
      });
      if (triggerEv.IsNullOrCancelled()) break;
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.Repeat.UpgradeValueBy(1m);
    DynamicVars[THRESHOLD_VAR].UpgradeValueBy(-1m);
  }
}

