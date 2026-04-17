using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
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
/// Collect. Draw 1 card. When drawn, trigger 2 (4) Auto Burst.
/// </summary>
public class Soulmate() : KahoCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [LinkuraKeywords.Collect];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new TriggerAutoBurstVar(2),
    new CardsVar(1),
  ];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromPower<AutoBurstPower>(),
    BurstHeartsVar.HoverTip(),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.CollectHearts(this, ctx);
    await CommonActions.Draw(this, ctx);
  }

  public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw) {
    if (card == this) {
      if (!CanTrigger()) return;
      IncrementTriggerCount();
      await Cmd.Wait(0.5f);
      await LinkuraCardActions.AutoBurst(this, choiceContext);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.TriggerAutoBurst().UpgradeValueBy(2m);
  }
}
