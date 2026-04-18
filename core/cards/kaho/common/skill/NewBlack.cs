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
/// On play: Gain 4 (6) Block.
/// Backstage: for every 6 Burst Hearts, gain 4 (6) block. (Current: X)
/// </summary>
public class NewBlack() : KahoInHandTriggerCard(1, CardType.Skill, CardRarity.Common, TargetType.None) {
  private const int BURST_PER_TRIGGER = 6;
  private const string TRACKER_VAR = "NEW_BLACK_TRACKER";
  private const string BACKSTAGE_BLOCK_VAR = "BACKSTAGE_BLOCK";

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BlockVar(4, ValueProp.Move),
    new BlockVar(BACKSTAGE_BLOCK_VAR, 4, ValueProp.Move),
    new DynamicVar(TRACKER_VAR, 0),
  ];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [BurstHeartsVar.HoverTip()];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardBlock(this, play);
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
    if (ev.Player != Owner || ev.ActualAmount <= 0 || !CanTrigger()) return;
    DynamicVars[TRACKER_VAR].BaseValue += ev.ActualAmount;
    while (DynamicVars[TRACKER_VAR].IntValue >= BURST_PER_TRIGGER) {
      DynamicVars[TRACKER_VAR].BaseValue -= BURST_PER_TRIGGER;
      var triggerEv = await TriggerWithAction(ev.Context, async () => {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars[BACKSTAGE_BLOCK_VAR], null);
      });
      if (triggerEv.IsNullOrCancelled()) break;
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.Block.UpgradeValueBy(2m);
    DynamicVars[BACKSTAGE_BLOCK_VAR].UpgradeValueBy(2m);
  }
}
