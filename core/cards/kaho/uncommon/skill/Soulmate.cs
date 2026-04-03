using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Soulmate (命定之人) — Cost 0, Skill, Uncommon.
/// Collect. When drawn, trigger 1 (2) [gold]Auto Burst[/gold].
/// </summary>
public class Soulmate() : LinkuraCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  private const int BASE_BURST = 1;
  private const string BURST_VAR = "RURIMEGU-SOULMATE_BURST";

  public override IEnumerable<CardKeyword> CanonicalKeywords => [LinkuraKeywords.Collect];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DynamicVar(BURST_VAR, BASE_BURST),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.CollectHearts(this, ctx);
  }

  public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw) {
    if (card == this) {
      int bursts = DynamicVars[BURST_VAR].IntValue;
      for (int i = 0; i < bursts; i++) {
        await LinkuraCmd.BurstHearts(Owner, BASE_BURST, this);
      }
    }
  }

  protected override void OnUpgrade() {
    DynamicVars[BURST_VAR].UpgradeValueBy(1m);
  }
}
