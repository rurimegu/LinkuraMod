using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;
using STS2RitsuLib.Keywords;

namespace RuriMegu.Core.Cards.Kaho.Rare.Skill;

/// <summary>
/// Present, Past, Future 鈥?Cost 1, Skill, Rare.
/// Discard hand, draw same amount.
/// Backstage: every 15 (10) Burst, trigger this effect.
/// </summary>
public class PresentPastFuture() : KahoInHandTriggerCard(1, CardType.Skill, CardRarity.Rare, TargetType.None) {
  private const string TRACKER_VAR = "LINKURA_MOD_PPF_TRACKER";
  private const string THRESHOLD_VAR = "LINKURA_MOD_PPF_THRESHOLD";

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DynamicVar(THRESHOLD_VAR, 15m),
    new DynamicVar(TRACKER_VAR, 0m),
  ];

  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
    ModKeywordRegistry.CreateHoverTip(LinkuraKeywords.Collect),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.DiscardAndDraw(this, ctx);
  }

  protected override Task InitializeSubscriptions() {
    TrackSubscription(Events.Collect.SubscribeLate(OnCollectLate));
    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    DynamicVars[TRACKER_VAR].BaseValue = 0;
    return base.AfterCombatEnd(room);
  }

  private async Task OnCollectLate(Events.CollectEvent ev) {
    if (ev.Player != Owner || ev.Amount <= 0 || !CanTrigger()) return;

    DynamicVars[TRACKER_VAR].BaseValue += ev.Amount;
    int threshold = DynamicVars[THRESHOLD_VAR].IntValue;

    while (DynamicVars[TRACKER_VAR].IntValue >= threshold) {
      DynamicVars[TRACKER_VAR].BaseValue -= threshold;
      var triggerEv = await TriggerWithAction(ev.Context, () => {
        return LinkuraCardActions.DiscardAndDraw(this, ev.Context);
      });
      if (triggerEv.IsNullOrCancelled()) break;
    }
  }

  protected override void OnUpgrade() {
    DynamicVars[THRESHOLD_VAR].UpgradeValueBy(-5m);
  }
}
