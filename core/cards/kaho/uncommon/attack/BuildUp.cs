using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Attack;

/// <summary>
/// Build-up — Cost 1, Attack, Uncommon.
/// Deal 6 (9) damage. Draw 1 (2) card(s).
/// Backstage: every 3 Burst triggers grants +1 extra draw when this card is played. (Current: X)
/// </summary>
public class BuildUp() : InHandTriggerCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {
  private const int BURSTS_PER_EXTRA_DRAW = 3;
  private const string TRACKER_VAR = "BUILD_UP_TRACKER";
  private const string DRAW_PREVIEW_VAR = "BUILD_UP_DRAW";

  private int _burstCount;
  private Subscription _burstSubscription;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(6, ValueProp.Move),
    new CardsVar(1),
    new DynamicVar(TRACKER_VAR, 0),
    new DynamicVar(DRAW_PREVIEW_VAR, 1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int totalDraw = DynamicVars[DRAW_PREVIEW_VAR].IntValue;
    _burstCount %= BURSTS_PER_EXTRA_DRAW;

    await CommonActions.CardAttack(this, play.Target).Execute(ctx);
    await CardPileCmd.Draw(ctx, totalDraw, Owner);
    UpdateDisplayVars();
  }

  public override Task BeforeCombatStartLate() {
    _burstSubscription = Events.Burst.SubscribeLate(OnBurstHearts);
    UpdateDisplayVars();
    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    _burstSubscription?.Dispose();
    _burstSubscription = null;
    _burstCount = 0;
    UpdateDisplayVars();
    return Task.CompletedTask;
  }

  private async Task OnBurstHearts(Events.BurstEvent ev) {
    if (ev.Player != Owner || ev.ActualAmount <= 0 || !this.IsInHand()) return;

    var triggerEv = await TryTrigger(ev.Context);
    if (triggerEv.IsNullOrCancelled()) return;

    _burstCount++;
    UpdateDisplayVars();
    await AfterTrigger(triggerEv);
  }

  private void UpdateDisplayVars() {
    if (!IsMutable) return;

    int progress = _burstCount % BURSTS_PER_EXTRA_DRAW;
    int extraDraw = _burstCount / BURSTS_PER_EXTRA_DRAW;

    DynamicVars[TRACKER_VAR].BaseValue = progress;
    DynamicVars[DRAW_PREVIEW_VAR].BaseValue = DynamicVars.Cards.IntValue + extraDraw;
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(3m);
    DynamicVars.Cards.UpgradeValueBy(1m);
    UpdateDisplayVars();
  }

  protected override void AfterDowngraded() {
    UpdateDisplayVars();
  }
}
