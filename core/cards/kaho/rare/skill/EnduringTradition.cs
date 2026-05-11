using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Powers.Kaho;
using STS2RitsuLib.Keywords;

namespace RuriMegu.Core.Cards.Kaho.Rare.Skill;

/// <summary>
/// Enduring Tradition — Cost 0, Skill, Rare.
/// Apply Enduring Tradition Power. (Retain.)
/// </summary>
public class EnduringTradition() : KahoCard(0, CardType.Skill, CardRarity.Rare, TargetType.None) {
  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
    HoverTipFactory.FromPower<AutoBurstPower>(),
    BurstHeartsVar.HoverTip(),
    ModKeywordRegistry.CreateHoverTip(LinkuraKeywords.Collect),
  ];
  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await PowerCmd.Apply<EnduringTraditionPower>(Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    AddKeyword(CardKeyword.Retain);
  }
}
