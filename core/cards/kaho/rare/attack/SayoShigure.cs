using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Rare.Attack;

/// <summary>
/// Sayo-Shigure — Cost 1, Attack, Rare.
/// Deal 9 (13) damage to ALL enemies.
/// Backstage: whenever you Burst 10, Burst 2 (3). (Current: X)
/// </summary>
public class SayoShigure() : InHandTriggerCard(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) {
  private const int BURSTS_PER_TRIGGER = 10;
  private const string TRACKER_VAR = "SAYO_SHIGURE_TRACKER";

  private Subscription _burstSubscription;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(9, ValueProp.Move),
    new BurstHeartsVar(2),
    new DynamicVar(TRACKER_VAR, 0),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play).Execute(ctx);
  }

  public override Task BeforeCombatStartLate() {
    _burstSubscription = Events.Burst.SubscribeLate(OnBurstHearts);
    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    _burstSubscription?.Dispose();
    _burstSubscription = null;
    DynamicVars[TRACKER_VAR].BaseValue = 0;
    return Task.CompletedTask;
  }

  private async Task OnBurstHearts(Events.BurstEvent ev) {
    if (ev.Player != Owner || ev.ActualAmount <= 0) return;
    if (!this.IsInHand()) return;

    DynamicVars[TRACKER_VAR].BaseValue += ev.ActualAmount;

    while (DynamicVars[TRACKER_VAR].IntValue >= BURSTS_PER_TRIGGER) {
      var triggerEv = await TryTrigger(ev.Context);
      if (triggerEv.IsNullOrCancelled()) break;

      DynamicVars[TRACKER_VAR].BaseValue -= BURSTS_PER_TRIGGER;
      await LinkuraCardActions.BurstHearts(this, ev.Context);
      await AfterTrigger(triggerEv);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(4m);
    DynamicVars.BurstHearts().UpgradeValueBy(1m);
  }
}
