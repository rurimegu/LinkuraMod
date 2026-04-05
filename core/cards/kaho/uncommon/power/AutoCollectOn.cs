using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Power;

/// <summary>
/// Auto Collect: On (自动收心：On) — Cost 1 (0), Power, Uncommon.
/// Whenever your ❤️ hits maximum, Collect.
/// </summary>
public class AutoCollectOn() : LinkuraCard(1, CardType.Power, CardRarity.Uncommon, TargetType.None) {
  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromKeyword(LinkuraKeywords.Collect),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    if (HeartsState.ReachedMaxHearts(Owner)) {
      await LinkuraCmd.CollectHearts(Owner, ctx);
    }
    await PowerCmd.Apply<AutoCollectOnPower>(Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    EnergyCost.UpgradeBy(-1);
  }
}
