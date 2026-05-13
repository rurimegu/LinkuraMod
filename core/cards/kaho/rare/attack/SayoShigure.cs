using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Rare.Attack;

/// <summary>
/// Sayo-Shigure — Cost 1, Attack, Rare.
/// Deal 9 (13) damage to ALL enemies.
/// Backstage: whenever you Burst 9, Burst 2 (3). (Current: X)
/// </summary>
public class SayoShigure() : KahoInHandTriggerCard(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) {
  private const int BURSTS_PER_TRIGGER = 9;
  private const string TRACKER_VAR = "SAYO_SHIGURE_TRACKER";

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(9, ValueProp.Move),
    new BurstHeartsVar(2),
    new DynamicVar(TRACKER_VAR, 0),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play).Execute(ctx);
  }

  protected override Task InitializeSubscriptions() {
    TrackSubscription(Events.Burst.SubscribeLate(OnBurstHearts));
    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    DynamicVars[TRACKER_VAR].BaseValue = 0;
    return base.AfterCombatEnd(room);
  }

  private async Task OnBurstHearts(Events.BurstEvent ev) {
    if (ev.Player != Owner || ev.ActualAmount <= 0) return;
    if (!CanTrigger()) return;

    DynamicVars[TRACKER_VAR].BaseValue += ev.ActualAmount;

    while (DynamicVars[TRACKER_VAR].IntValue >= BURSTS_PER_TRIGGER) {
      DynamicVars[TRACKER_VAR].BaseValue -= BURSTS_PER_TRIGGER;
      var triggerEv = await TriggerWithAction(ev.Context, async () => {
        await LinkuraCardActions.BurstHearts(this, ev.Context);
      });
      if (triggerEv.IsNullOrCancelled()) break;
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(4m);
    DynamicVars.BurstHearts().UpgradeValueBy(1m);
  }
}
