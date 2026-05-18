using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Powers.Kaho;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Rare.Power;

/// <summary>
/// Hinoshita Install (日野下安装) — Cost 1 (0), Power, Rare.
/// When another player plays a card, it also trigger your Auto Burst.
/// </summary>
public class HinoshitaInstall() : KahoCard(1, CardType.Power, CardRarity.Rare, TargetType.None) {
  public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
    HoverTipFactory.FromPower<AutoBurstPower>(),
    BurstHeartsVar.HoverTip(),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await Owner.PlayCastAnim();
    await PowerCmd.Apply<HinoshitaInstallPower>(Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    EnergyCost.UpgradeBy(-1);
  }
}
