using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Spring Breeze Swing (春风秋千) — Cost 1, Skill, Common.
/// Draw 1 (2) card.
/// Backstage: whenever Burst Hearts reaches the ♥ limit, gain 1 energy.
/// </summary>
public class SpringBreezeSwing() : InHandTriggerCard(1, CardType.Skill, CardRarity.Common, TargetType.None) {
  private Subscription _burstSubscription;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CardsVar(1),
    new EnergyVar(1),
  ];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [BurstHeartsVar.HoverTip()];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.Draw(this, ctx);
  }

  public override Task BeforeCombatStartLate() {
    _burstSubscription = Events.Burst.SubscribeLate(OnBurstHearts);
    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    _burstSubscription?.Dispose();
    _burstSubscription = null;
    return Task.CompletedTask;
  }

  private async Task OnBurstHearts(Events.BurstEvent ev) {
    if (ev.Player != Owner || ev.ActualAmount <= 0) return;
    // Trigger only if hearts are now at their maximum (burst just filled to cap).
    int currentHearts = HeartsState.GetHearts(Owner);
    int maxHearts = HeartsState.GetMaxHearts(Owner);
    if (currentHearts < maxHearts) return;
    await TriggerWithAction(ev.Context, () => PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner));
  }

  protected override void OnUpgrade() {
    DynamicVars.Cards.UpgradeValueBy(1m);
  }
}
