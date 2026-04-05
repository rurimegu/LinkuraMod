using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Nεw Black — Cost 1, Skill, Common.
/// On play: Gain 6 (9) Block.
/// Backstage: for every 6 Burst Hearts, gain 3 (4) block. (Current: X)
/// </summary>
public class NewBlack() : InHandTriggerCard(1, CardType.Skill, CardRarity.Common, TargetType.None) {
  private const int BURST_PER_TRIGGER = 6;
  private Subscription _burstSubscription;
  private const string TRACKER_VAR = "NEW_BLACK_TRACKER";
  private const string BACKSTAGE_BLOCK_VAR = "BACKSTAGE_BLOCK";

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BlockVar(6, ValueProp.Move),
    new DynamicVar(BACKSTAGE_BLOCK_VAR, 4),
    new DynamicVar(TRACKER_VAR, 0),
  ];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [BurstHeartsVar.HoverTip()];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardBlock(this, play);
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
    if (ev.Player != Owner || ev.ActualAmount <= 0 || !CanTrigger()) return;
    DynamicVars[TRACKER_VAR].BaseValue += ev.ActualAmount;
    while (DynamicVars[TRACKER_VAR].IntValue >= BURST_PER_TRIGGER) {
      int newTrackerVar = DynamicVars[TRACKER_VAR].IntValue - BURST_PER_TRIGGER;
      var triggerEv = await TriggerWithAction(ev.Context, async () => {
        DynamicVars[TRACKER_VAR].BaseValue = newTrackerVar;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars[BACKSTAGE_BLOCK_VAR].IntValue, ValueProp.Move, null);
      });
      if (triggerEv == null) break;
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.Block.UpgradeValueBy(3m);
    DynamicVars[BACKSTAGE_BLOCK_VAR].UpgradeValueBy(2m);
  }
}
