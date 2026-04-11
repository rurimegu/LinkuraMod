using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Powers.Kaho;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Power;

/// <summary>
/// Career Survey (杩涜矾璋冩煡) 鈥?Cost 1 (0), Power, Uncommon.
/// At the start of your turn, convert 鉂わ笍 to equal Block.
/// </summary>
public class CareerSurvey() : KahoCard(1, CardType.Power, CardRarity.Uncommon, TargetType.None) {
  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.Static(StaticHoverTip.Block),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await PowerCmd.Apply<CareerSurveyPower>(Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    EnergyCost.UpgradeBy(-1);
  }
}
