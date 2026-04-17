using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Suki Suki Club (喜欢喜欢俱乐部) — Cost 1, Skill, Uncommon.
/// Draw 2 (3) cards. When drawn, increase max ❤️ by 2 (3).
/// </summary>
public class SukiSukiClub() : KahoCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CardsVar(2),
    new ExpandHeartsVar(2),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.Draw(this, ctx);
  }

  public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw) {
    if (card == this) {
      if (!CanTrigger()) return;
      IncrementTriggerCount();
      await Cmd.Wait(0.5f);
      int heartBoost = DynamicVars.ExpandHearts().IntValue;
      await LinkuraCmd.IncreaseMaxHearts(Owner, choiceContext, heartBoost, this);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.Cards.UpgradeValueBy(1m);
    DynamicVars.ExpandHearts().UpgradeValueBy(1m);
  }
}
