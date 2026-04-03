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
  public override IEnumerable<CardKeyword> CanonicalKeywords => [LinkuraKeywords.Collect];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BurstHeartsVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.CollectHearts(this, ctx);
  }

  public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw) {
    if (card == this) {
      int bursts = DynamicVars.BurstHearts().IntValue;
      for (int i = 0; i < bursts; i++) {
        await LinkuraCmd.TriggerAutoBurst(Owner, choiceContext, this);
      }
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.BurstHearts().UpgradeValueBy(1m);
  }
}
