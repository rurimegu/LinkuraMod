using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Celebration! — Cost 1, Skill, Uncommon.
/// Draw 1(2) cards. Backstage: every 5 (4) times you Burst, draw 1 card. (Current: X)
/// </summary>
public class Celebration() : InHandTriggerCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  private const string TRACKER_VAR = "CELEBRATION_TRACKER";
  private const string THRESHOLD_VAR = "CELEBRATION_THRESHOLD";
  private Subscription _burstSubscription;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CardsVar(1),
    new DynamicVar(TRACKER_VAR, 0),
    new DynamicVar(THRESHOLD_VAR, 4),
  ];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    BurstHeartsVar.HoverTip(),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.Draw(this, ctx);
  }

  public override Task BeforeCombatStartLate() {
    _burstSubscription = Events.Burst.SubscribeVeryEarly(OnBurstHearts);

    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    _burstSubscription?.Dispose();
    _burstSubscription = null;
    return Task.CompletedTask;
  }

  private async Task OnBurstHearts(Events.BurstEvent ev) {
    if (ev.Player != Owner || !CanTrigger()) return;
    DynamicVars[TRACKER_VAR].BaseValue++;

    int threshold = DynamicVars[THRESHOLD_VAR].IntValue;
    while (DynamicVars[TRACKER_VAR].IntValue >= threshold) {
      int newTrackerVar = DynamicVars[TRACKER_VAR].IntValue - threshold;
      var triggerEv = await TriggerWithAction(ev.Context, async () => {
        DynamicVars[TRACKER_VAR].BaseValue = newTrackerVar;
        await CardPileCmd.Draw(ev.Context, Owner);
      });
      if (triggerEv == null) break;
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.Cards.UpgradeValueBy(1m);
    DynamicVars[THRESHOLD_VAR].UpgradeValueBy(-1m);
  }
}
