using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RuriMegu.Core.Powers.Kaho;
using RuriMegu.Core.Utils;
using STS2RitsuLib.Keywords;

namespace RuriMegu.Core.Cards.Kaho.Common.Power;

/// <summary>
/// Section Change (小节变换) — Cost 1, Power, Common.
/// At the end of your turn, Collect. (Innate.)
/// </summary>
public class SectionChange() : KahoCard(1, CardType.Power, CardRarity.Common, TargetType.None) {
  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
    ModKeywordRegistry.CreateHoverTip(LinkuraKeywords.Collect),
  ];


  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await Owner.PlayCastAnim();
    await PowerCmd.Apply<SectionChangePower>(Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    AddKeyword(CardKeyword.Innate);
    EnergyCost.UpgradeBy(-1);
  }
}
