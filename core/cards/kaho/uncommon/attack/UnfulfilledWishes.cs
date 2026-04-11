using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Attack;

/// <summary>
/// Unfulfilled Wishes 鈥?Cost 2, Attack, Uncommon.
/// Deal damage equal to (max 鉂わ笍 - current 鉂わ笍). Gain Block equal to current 鉂わ笍.
/// Upgraded: gains Retain.
/// </summary>
public class UnfulfilledWishes() : KahoCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {
  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.Static(StaticHoverTip.Block),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int hearts = HeartsState.GetHearts(Owner);
    int maxHearts = HeartsState.GetMaxHearts(Owner);
    int damage = maxHearts - hearts;

    if (damage > 0) {
      await DamageCmd.Attack(damage)
        .FromCard(this)
        .Targeting(play.Target)
        .Execute(ctx);
    }

    if (hearts > 0) {
      await CreatureCmd.GainBlock(Owner.Creature, hearts, ValueProp.Move, play);
    }
  }

  protected override void OnUpgrade() {
    AddKeyword(CardKeyword.Retain);
  }
}
