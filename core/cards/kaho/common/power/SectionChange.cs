using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Powers.Kaho;

namespace RuriMegu.Core.Cards.Kaho.Common.Power;

/// <summary>
/// Section Change (灏忚妭鍙樻崲) 鈥?Cost 1, Power, Common.
/// At the end of your turn, Collect. (Innate.)
/// </summary>
public class SectionChange() : KahoCard(1, CardType.Power, CardRarity.Common, TargetType.None) {
  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromKeyword(LinkuraKeywords.Collect),
  ];


  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await PowerCmd.Apply<SectionChangePower>(Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    AddKeyword(CardKeyword.Innate);
    EnergyCost.UpgradeBy(-1);
  }
}
