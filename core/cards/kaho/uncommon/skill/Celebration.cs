using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Celebration! — Cost 1, Skill, Uncommon.
/// Draw 1 card. Backstage: for every 5 (4) Burst Hearts, draw 1 card. (Current: X) (Innate.)
/// </summary>
public class Celebration() : InHandTriggerCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  private const string TRACKER_VAR = "CELEBRATION_TRACKER";
  private const string THRESHOLD_VAR = "CELEBRATION_THRESHOLD";
  private int _burstAccumulated;
  private Subscription _burstSubscription;

  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CardsVar(1),
    new DynamicVar(TRACKER_VAR, 0),
    new DynamicVar(THRESHOLD_VAR, 5),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.Draw(this, ctx);
  }

  public override Task BeforeCombatStartLate() {
    _burstSubscription = Events.BurstHearts.SubscribeLate(OnBurstHearts);
    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    _burstSubscription?.Dispose();
    return Task.CompletedTask;
  }

  private async Task OnBurstHearts(Events.BurstHeartsEvent ev) {
    if (ev.Player != Owner) return;
    _burstAccumulated += ev.ActualAmount;
    DynamicVars[TRACKER_VAR].BaseValue = _burstAccumulated;

    int threshold = DynamicVars[THRESHOLD_VAR].IntValue;
    while (_burstAccumulated >= threshold) {
      var triggerEv = await TryTrigger();
      if (triggerEv.IsNullOrCancelled()) break;
      _burstAccumulated -= threshold;
      DynamicVars[TRACKER_VAR].BaseValue = _burstAccumulated;
      // Draw 1 card as part of the trigger
      await CommonActions.Draw(this, null);
      await AfterTrigger(triggerEv);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars[THRESHOLD_VAR].UpgradeValueBy(-1m);
  }
}
