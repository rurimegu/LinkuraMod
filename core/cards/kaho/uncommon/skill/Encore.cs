using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
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
/// Encore (安可) — Cost 1 (0), Skill, Uncommon.
/// Gain 1 Block for every Burst triggered this turn. (Current: X) (Innate. Retain.)
/// </summary>
public class Encore() : LinkuraCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  private const string TRACKER_VAR = "ENCORE_TRACKER";
  private Subscription _burstSubscription;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DynamicVar(TRACKER_VAR, 0),
  ];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    BurstHeartsVar.HoverTip(),
    HoverTipFactory.Static(StaticHoverTip.Block),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int block = DynamicVars[TRACKER_VAR].IntValue;
    if (block > 0) {
      await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, play);
    }
  }

  public override Task BeforeSideTurnStart(PlayerChoiceContext ctx, CombatSide side, CombatState combatState) {
    if (side == Owner.Creature.Side) {
      DynamicVars[TRACKER_VAR].BaseValue = 0;
    }
    return base.BeforeSideTurnStart(ctx, side, combatState);
  }

  public override Task BeforeCombatStartLate() {
    _burstSubscription = Events.Burst.SubscribeVeryEarly(OnBurstHearts);
    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    _burstSubscription?.Dispose();
    _burstSubscription = null;
    DynamicVars[TRACKER_VAR].BaseValue = 0;
    return Task.CompletedTask;
  }

  private async Task OnBurstHearts(Events.BurstEvent ev) {
    if (ev.Player != Owner) return;
    DynamicVars[TRACKER_VAR].BaseValue++;
  }

  protected override void OnUpgrade() {
    AddKeyword(CardKeyword.Innate);
    AddKeyword(CardKeyword.Retain);
  }
}
