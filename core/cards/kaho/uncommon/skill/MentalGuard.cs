using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RuriMegu.Core.Powers;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Mental Guard (精神守护) — Cost 1, Skill, Uncommon.
/// This turn, Burst Hearts grant Block instead of hearts.
/// </summary>
public class MentalGuard() : LinkuraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    BurstHeartsVar.HoverTip(),
    HoverTipFactory.Static(StaticHoverTip.Block),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await PowerCmd.Apply<MentalGuardPower>(Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    RemoveKeyword(CardKeyword.Ethereal);
  }
}
