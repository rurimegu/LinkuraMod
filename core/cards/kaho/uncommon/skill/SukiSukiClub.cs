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
/// Suki Suki Club (喜欢喜欢俱乐部) — Cost 1, Skill, Uncommon.
/// Draw 2 (3) cards. When drawn, increase max ❤️ by 2 (3).
/// </summary>
public class SukiSukiClub() : LinkuraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  private const int BASE_HEART_INCREASE = 2;
  private const string HEART_BOOST_VAR = "RURIMEGU-SUKI_HEART_BOOST";

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CardsVar(1),
    new DynamicVar(HEART_BOOST_VAR, BASE_HEART_INCREASE),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.Draw(this, ctx);
  }

  public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw) {
    if (card == this) {
      int heartBoost = DynamicVars[HEART_BOOST_VAR].IntValue;
      await LinkuraCmd.IncreaseMaxHearts(Owner, heartBoost, this);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.Cards.UpgradeValueBy(1m);
    DynamicVars[HEART_BOOST_VAR].UpgradeValueBy(1m);
  }
}
