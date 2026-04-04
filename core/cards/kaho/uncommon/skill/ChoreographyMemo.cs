using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Choreography Memo (舞步编排) — Cost 1, Skill, Uncommon.
/// On play: Burst 9 (12).
/// Backstage: for every 3 (2) cards you play, Collect. (Current: X)
/// </summary>
public class ChoreographyMemo() : InHandTriggerCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  private const string TRACKER_VAR = "CHOREOGRAPHY_TRACKER";
  private const string THRESHOLD_VAR = "CHOREOGRAPHY_THRESHOLD";

  public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Append(LinkuraKeywords.Collect);

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BurstHeartsVar(9),
    new DynamicVar(TRACKER_VAR, 0),
    new DynamicVar(THRESHOLD_VAR, 3),
  ];
  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.BurstHearts(this, ctx);
  }

  public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay) {
    await base.AfterCardPlayed(context, cardPlay);
    if (cardPlay.Card.Owner != Owner || cardPlay.Card == this || !CanTrigger()) return;

    DynamicVars[TRACKER_VAR].BaseValue++;

    int threshold = DynamicVars[THRESHOLD_VAR].IntValue;
    while (DynamicVars[TRACKER_VAR].IntValue >= threshold) {
      int newTrackerVar = DynamicVars[TRACKER_VAR].IntValue - threshold;
      var triggerEv = await TriggerWithAction(context, async () => {
        DynamicVars[TRACKER_VAR].BaseValue = newTrackerVar;
        await LinkuraCardActions.CollectHearts(this, context);
      });
      if (triggerEv == null) break;
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.BurstHearts().UpgradeValueBy(3m);
    DynamicVars[THRESHOLD_VAR].UpgradeValueBy(-1m);
  }
}

