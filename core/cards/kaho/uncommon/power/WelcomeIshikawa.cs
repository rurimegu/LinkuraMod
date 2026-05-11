using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Powers.Kaho;
using RuriMegu.Core.Utils;
using STS2RitsuLib.Keywords;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Power;

/// <summary>
/// Welcome! Ishikawa (石川大观光) — Cost 1, Power, Uncommon.
/// Start of turn, choose 1 (2) Backstage cards from draw pile to hand.
/// Innate.
/// </summary>
public class WelcomeIshikawa() : KahoCard(1, CardType.Power, CardRarity.Uncommon, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate];
  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
    ModKeywordRegistry.CreateHoverTip(LinkuraKeywords.Backstage),
  ];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CardsVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await Owner.PlayCastAnim();
    await PowerCmd.Apply<WelcomeIshikawaPower>(Owner.Creature, DynamicVars.Cards.IntValue, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.Cards.UpgradeValueBy(1m);
  }
}
