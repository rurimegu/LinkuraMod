using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Nεw Black — Cost 1, Skill, Common.
/// Draw 1 card.
/// Backstage: for every 6 Burst Hearts, gain 3 (4) block.
/// </summary>
public class NewBlack() : InHandTriggerCard(1, CardType.Skill, CardRarity.Common, TargetType.None) {
  private const int BURST_PER_TRIGGER = 6;
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
    if (ev.Player != Owner || ev.ActualAmount <= 0 || !this.IsInHand()) return;
    DynamicVars[TRACKER_VAR].BaseValue += ev.ActualAmount;
    while (DynamicVars[TRACKER_VAR].IntValue >= BURST_PER_TRIGGER) {
      var triggerEv = await TryTrigger(ev.Context);
      if (triggerEv.IsNullOrCancelled()) return;
      DynamicVars[TRACKER_VAR].BaseValue -= BURST_PER_TRIGGER;
      await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.IntValue, ValueProp.Move, null);
      await AfterTrigger(triggerEv);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.Block.UpgradeValueBy(1m);
  }
}
