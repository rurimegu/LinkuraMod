using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho;

/// <summary>
/// Unfulfilled Wishes — Cost 2, Attack, Uncommon.
/// Deal damage equal to (max ❤️ - current ❤️). Gain Block equal to current ❤️.
/// </summary>
public class UnfulfilledWishes() : LinkuraCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {
  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int hearts = HeartsState.GetHearts(Owner);
    int maxHearts = HeartsState.GetMaxHearts(Owner);
    int damage = maxHearts - hearts;

    await DamageCmd.Attack(damage)
      .FromCard(this)
      .Targeting(play.Target)
      .Execute(ctx);

    await CreatureCmd.GainBlock(Owner.Creature, hearts, ValueProp.Move, play);
  }
}
