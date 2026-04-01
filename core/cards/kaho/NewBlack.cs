using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho;

/// <summary>
/// Nεw Black — Cost 1, Skill, Common.
/// Draw 1 card.
/// Backstage: for every 6 Burst Hearts, gain 3 (4) block.
/// </summary>
public class NewBlack() : InHandTriggerCard(1, CardType.Skill, CardRarity.Common, TargetType.None) {
  private const int BURST_PER_TRIGGER = 6;
  private int _burstAccumulated;
  private Subscription _burstSubscription;
  private const string TRACKER_VAR = "NEW_BLACK_TRACKER";

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CardsVar(1),
    new BlockVar(3, ValueProp.Move),
    new DynamicVar(TRACKER_VAR, 0),
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
    _burstSubscription = null;
    _burstAccumulated = 0;
    UpdateTracker();
    return Task.CompletedTask;
  }

  private async Task OnBurstHearts(Events.BurstHeartsEvent ev) {
    if (ev.Player != Owner || ev.ActualAmount <= 0) return;
    _burstAccumulated += ev.ActualAmount;
    UpdateTracker();
    while (_burstAccumulated >= BURST_PER_TRIGGER) {
      var triggerEv = await TryTrigger();
      if (triggerEv.IsNullOrCancelled()) return;
      _burstAccumulated -= BURST_PER_TRIGGER;
      UpdateTracker();
      await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.IntValue, ValueProp.Move, null);
      await AfterTrigger(triggerEv);
    }
  }

  private void UpdateTracker() {
    if (!IsMutable) return;
    DynamicVars[TRACKER_VAR].BaseValue = _burstAccumulated;
  }

  protected override void OnUpgrade() {
    DynamicVars.Block.UpgradeValueBy(1m);
  }
}
