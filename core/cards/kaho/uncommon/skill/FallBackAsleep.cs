using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Fall Back Asleep (回笼觉) — Cost 2 (1), Skill, Uncommon.
/// Gain Block equal to current ❤️. Block persists this turn. Exhaust.
/// </summary>
public class FallBackAsleep() : LinkuraCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int hearts = HeartsState.GetHearts(Owner);
    await CreatureCmd.GainBlock(Owner.Creature, hearts, ValueProp.Move, play);
    // Note: Block persistence would require a custom power implementation
  }

  protected override void OnUpgrade() {
    EnergyCost.UpgradeBy(-1);
  }
}
