using System.Collections.Generic;
using System.Linq;
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
/// Gain block equal to current max ♥. Exhaust. Upgrade: Remove Exhaust.
/// </summary>
public class BunnyDefend() : KahoCard(2, CardType.Skill, CardRarity.Common, TargetType.None) {
  protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];
  public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Append(CardKeyword.Exhaust);
  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int maxHearts = HeartsState.GetMaxHearts(Owner);
    await CreatureCmd.GainBlock(Owner.Creature, maxHearts, ValueProp.Move, play);
  }

  protected override void OnUpgrade() {
    RemoveKeyword(CardKeyword.Exhaust);
  }
}
