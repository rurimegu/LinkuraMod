using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Soulmate (命定之人) — Cost 0, Skill, Uncommon.
/// Collect. When drawn, trigger 1 (2) [gold]Auto Burst[/gold].
/// </summary>
public class Soulmate() : LinkuraCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [LinkuraKeywords.Collect];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new TriggerAutoBurstVar(1),
  ];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromPower<AutoBurstPower>(),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.CollectHearts(this, ctx);
  }

  public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw) {
    if (card == this) {
      await LinkuraCardActions.AutoBurst(this, choiceContext);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.TriggerAutoBurst().UpgradeValueBy(1m);
  }
}
