using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Attack;

/// <summary>
/// Training Outcomes — Cost 4 (3), Attack, Common.
/// Deal 12 (16) damage.
/// Backstage: whenever you Collect, this card costs 1 less in this combat.
/// </summary>
public class TrainingOutcomes() : InHandTriggerCard(4, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) {
  private Subscription _collectHeartsSubscription;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(12, ValueProp.Move),
    new EnergyVar(1),
  ];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => base.ExtraHoverTips.Append(
    HoverTipFactory.FromKeyword(LinkuraKeywords.Collect));

  // Override BeforeCombatStartLate to subscribe to CollectHearts events.
  public override Task BeforeCombatStartLate() {
    _collectHeartsSubscription = Events.Collect.SubscribeLate(OnCollectHearts);
    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    _collectHeartsSubscription?.Dispose();
    _collectHeartsSubscription = null;
    return Task.CompletedTask;
  }

  private async Task OnCollectHearts(Events.CollectEvent ev) {
    if (ev.Player != Owner) return;
    await TriggerWithAction(ev.Context, () => {
      EnergyCost.AddThisCombat(-1, true);
      InvokeEnergyCostChanged();
      return Task.CompletedTask;
    });
  }

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play.Target).Execute(ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(4m);
    EnergyCost.UpgradeBy(-1);
  }
}
