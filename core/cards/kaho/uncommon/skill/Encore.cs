using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Encore (安可) — Cost 1, Skill, Uncommon.
/// This turn, gain 1 Block for every Burst Heart triggered. (Current: X) (Innate. Retain.)
/// </summary>
public class Encore() : LinkuraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  private const string TRACKER_VAR = "ENCORE_TRACKER";
  private Subscription _burstSubscription;

  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate, CardKeyword.Retain];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DynamicVar(TRACKER_VAR, 0),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    // Gain block on burst during this turn
    await Task.CompletedTask;
  }

  public override Task BeforeCombatStartLate() {
    _burstSubscription = Events.BurstHearts.SubscribeLate(OnBurstHearts);
    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    _burstSubscription?.Dispose();
    return Task.CompletedTask;
  }

  private async Task OnBurstHearts(Events.BurstHeartsEvent ev) {
    if (ev.Player != Owner) return;
    DynamicVars[TRACKER_VAR].BaseValue = DynamicVars[TRACKER_VAR].IntValue + ev.ActualAmount;
  }

  protected override void OnUpgrade() {
    // No upgrade changes
  }
}
