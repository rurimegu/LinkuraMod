using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Encore (安可) — Cost 0, Skill, Uncommon.
/// Gain 2 Block for every Burst triggered this turn. (Current: X) (Retain.)
/// </summary>
public class Encore() : KahoCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  private const string TRACKER_VAR = "ENCORE_TRACKER";

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DynamicVar(TRACKER_VAR, 0),
    new BlockVar(2, ValueProp.Move),
  ];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    BurstHeartsVar.HoverTip(),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int times = DynamicVars[TRACKER_VAR].IntValue;
    for (int i = 0; i < times; i++) {
      await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }
  }

  public override Task BeforeSideTurnStart(PlayerChoiceContext ctx, CombatSide side, CombatState combatState) {
    if (side == Owner.Creature.Side) {
      DynamicVars[TRACKER_VAR].BaseValue = 0;
    }
    return base.BeforeSideTurnStart(ctx, side, combatState);
  }

  protected override Task InitializeSubscriptions() {
    TrackSubscription(Events.Burst.SubscribeLate(OnBurstHearts));
    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    DynamicVars[TRACKER_VAR].BaseValue = 0;
    return base.AfterCombatEnd(room);
  }

  private Task OnBurstHearts(Events.BurstEvent ev) {
    if (ev.Player != Owner || ev.ActualAmount <= 0) return Task.CompletedTask;
    DynamicVars[TRACKER_VAR].BaseValue++;
    return Task.CompletedTask;
  }

  protected override void OnUpgrade() {
    AddKeyword(CardKeyword.Retain);
  }
}
