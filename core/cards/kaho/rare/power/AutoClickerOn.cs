using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using RuriMegu.Core.Powers;

namespace RuriMegu.Core.Cards.Kaho.Rare.Power;

/// <summary>
/// Auto Clicker: On (连点器：On) — Cost 2 (1), Power, Rare.
/// Whenever you Burst, Collect. All enemies gain 99 Intangible.
/// </summary>
public class AutoClickerOn() : LinkuraCard(2, CardType.Power, CardRarity.Rare, TargetType.None) {

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
