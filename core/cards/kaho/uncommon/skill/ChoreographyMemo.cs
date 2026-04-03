using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Choreography Memo (舞步编排) — Cost 1, Skill, Uncommon.
/// Draw 1 card. Backstage: for every 3 (2) cards you play, Collect. (Current: X)
/// </summary>
public class ChoreographyMemo() : InHandTriggerCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  private const string TRACKER_VAR = "CHOREOGRAPHY_TRACKER";
  private const string THRESHOLD_VAR = "CHOREOGRAPHY_THRESHOLD";
  private int _cardsPlayedCount;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CardsVar(1),
    new DynamicVar(TRACKER_VAR, 0),
    new DynamicVar(THRESHOLD_VAR, 3),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.Draw(this, ctx);
  }

  public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay) {
    await base.AfterCardPlayed(context, cardPlay);
    if (cardPlay.Card.Owner != Owner) return;

    _cardsPlayedCount++;
    DynamicVars[TRACKER_VAR].BaseValue = _cardsPlayedCount;

    int threshold = DynamicVars[THRESHOLD_VAR].IntValue;
    while (_cardsPlayedCount >= threshold) {
      var triggerEv = await TryTrigger();
      if (triggerEv.IsNullOrCancelled()) break;
      _cardsPlayedCount -= threshold;
      DynamicVars[TRACKER_VAR].BaseValue = _cardsPlayedCount;
      await LinkuraCardActions.CollectHearts(this, context);
      await AfterTrigger(triggerEv);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars[THRESHOLD_VAR].UpgradeValueBy(-1m);
  }
}

