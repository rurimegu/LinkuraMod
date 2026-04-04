using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Bunny Defend — Cost 2, Skill, Common.
/// Gain block equal to current max ♥. (Retain.)
/// </summary>
public class BunnyDefend() : LinkuraCard(2, CardType.Skill, CardRarity.Common, TargetType.None) {
  protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];
  protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int maxHearts = HeartsState.GetMaxHearts(Owner);
    await CreatureCmd.GainBlock(Owner.Creature, maxHearts, ValueProp.Move, play);
  }

  protected override void OnUpgrade() {
    AddKeyword(CardKeyword.Retain);
  }
}
