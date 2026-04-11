using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Powers.Kaho;

namespace RuriMegu.Core.Cards.Kaho.Rare.Power;

/// <summary>
/// Auto Clicker: On (杩炵偣鍣細On) 鈥?Cost 1 (0), Power, Rare.
/// Whenever you Burst, Collect. All enemies gain 99 Intangible.
/// </summary>
public class AutoClickerOn() : KahoCard(1, CardType.Power, CardRarity.Rare, TargetType.None) {

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    BurstHeartsVar.HoverTip(),
    HoverTipFactory.FromKeyword(LinkuraKeywords.Collect),
    HoverTipFactory.FromPower<IntangiblePower>(),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await PowerCmd.Apply<AutoClickerOnPower>(Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    EnergyCost.UpgradeBy(-1);
  }
}
