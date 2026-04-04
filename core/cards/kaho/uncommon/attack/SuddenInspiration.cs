using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Attack;

/// <summary>
/// Sudden Inspiration — Cost 1, Attack, Uncommon.
/// Deal 6 (9) damage. Collect when drawn.
/// </summary>
public class SuddenInspiration() : LinkuraCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {

  public override IEnumerable<CardKeyword> CanonicalKeywords => [LinkuraKeywords.Collect];
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(6, ValueProp.Move),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play.Target).Execute(ctx);
  }

  public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw) {
    if (card == this) {
      await Cmd.Wait(0.25f);
      await LinkuraCardActions.CollectHearts(this, choiceContext);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(3m);
  }
}
