using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RuriMegu.Core.Powers;

namespace RuriMegu.Core.Cards.Kaho.Rare.Power;

/// <summary>
/// Flower Knot (花结) — Cost 1, Power, Rare, (Retain.)
/// Whenever you trigger a Backstage effect, trigger Auto Burst once.
/// </summary>
public class FlowerKnot() : LinkuraCard(1, CardType.Power, CardRarity.Rare, TargetType.None) {

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromKeyword(LinkuraKeywords.Backstage),
    AutoBurstVar.HoverTip(),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await PowerCmd.Apply<FlowerKnotPower>(Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    AddKeyword(CardKeyword.Retain);
  }
}
