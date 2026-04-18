using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Powers.Kaho;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Rare.Power;

/// <summary>
/// Flower Knot (花结) — Cost 1, Power, Rare.
/// Whenever you trigger a Backstage effect, discard that backstage card and draw 1 (2) cards.
/// </summary>
public class FlowerKnot() : KahoCard(1, CardType.Power, CardRarity.Rare, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromKeyword(LinkuraKeywords.Backstage),
  ];
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CardsVar(1),
  ];


  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await Owner.PlayCastAnim();
    await PowerCmd.Apply<FlowerKnotPower>(Owner.Creature, DynamicVars.Cards.IntValue, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.Cards.UpgradeValueBy(1m);
  }
}
