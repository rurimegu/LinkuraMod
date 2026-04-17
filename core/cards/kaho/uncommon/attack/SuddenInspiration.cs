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
/// Deal 8 (12) damage. Collect when drawn.
/// </summary>
public class SuddenInspiration() : KahoCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {

  public override IEnumerable<CardKeyword> CanonicalKeywords => [LinkuraKeywords.Collect];
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(8, ValueProp.Move),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play.Target).Execute(ctx);
  }

  public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw) {
    if (card == this) {
      if (!CanTrigger()) return;
      IncrementTriggerCount();
      await Cmd.Wait(0.5f);
      await LinkuraCardActions.CollectHearts(this, choiceContext);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(4m);
  }
}
