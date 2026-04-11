using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Fall Back Asleep (鍥炵瑙? 鈥?Cost 2 (1), Skill, Uncommon.
/// Gain Block equal to current 鉂わ笍. Block persists this turn. Exhaust.
/// </summary>
public class FallBackAsleep() : KahoCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.Static(StaticHoverTip.Block)
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int hearts = HeartsState.GetHearts(Owner);
    await CreatureCmd.GainBlock(Owner.Creature, hearts, ValueProp.Move, play);
    if (!Owner.Creature.HasPower<BlurPower>()) {
      await PowerCmd.Apply<BlurPower>(Owner.Creature, 1, Owner.Creature, this);
    }
  }

  protected override void OnUpgrade() {
    EnergyCost.UpgradeBy(-1);
  }
}
