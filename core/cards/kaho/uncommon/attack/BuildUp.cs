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

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Attack;

/// <summary>
/// Build-up — Cost 1, Attack, Uncommon.
/// Deal 8 (12) damage. Draw 1 (2) card(s).
/// Backstage: every 3 Burst triggers grants +1 extra draw when this card is played. (Current: X)
/// </summary>
public class BuildUp() : KahoInHandTriggerCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {
  private const string DRAW_PREVIEW_VAR = "BUILD_UP_DRAW";
  private const string TRACKER_VAR = "BUILD_UP_TRACKER";

  private int _bonusDraws;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(8, ValueProp.Move),
    new CalculationBaseVar(1),
    new CalculationExtraVar(1),
    new CalculatedVar(DRAW_PREVIEW_VAR).WithMultiplier(
      static (card, _) => (card as BuildUp)?._bonusDraws ?? 0),
    new DynamicVar(TRACKER_VAR, 0),
  ];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [BurstHeartsVar.HoverTip()];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int totalDraw = (int)((CalculatedVar)DynamicVars[DRAW_PREVIEW_VAR]).Calculate(null);

    await CommonActions.CardAttack(this, play.Target).Execute(ctx);
    await CardPileCmd.Draw(ctx, totalDraw, Owner);
    DynamicVars[TRACKER_VAR].BaseValue = 0;
    _bonusDraws = 0;
  }

  protected override Task InitializeSubscriptions() {
    TrackSubscription(Events.Burst.SubscribeLate(OnBurstHearts));
    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    DynamicVars[TRACKER_VAR].BaseValue = 0;
    _bonusDraws = 0;
    return base.AfterCombatEnd(room);
  }

  private async Task OnBurstHearts(Events.BurstEvent ev) {
    if (ev.Player != Owner || ev.ActualAmount <= 0 || !CanTrigger()) return;

    await TriggerWithAction(ev.Context, () => {
      DynamicVars[TRACKER_VAR].BaseValue++;
      if (DynamicVars[TRACKER_VAR].IntValue >= 3) {
        DynamicVars[TRACKER_VAR].BaseValue -= 3;
        _bonusDraws++;
      }
      return Task.CompletedTask;
    });
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(4m);
    DynamicVars.CalculationBase.UpgradeValueBy(1m);
  }
}
