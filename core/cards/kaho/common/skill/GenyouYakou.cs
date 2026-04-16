using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Genyou Yakou (眩耀夜行) — Cost 0, Skill, Common.
/// If you have 6 or fewer ♥, gain 1 energy and draw 2 (3) cards. Ethereal.
/// </summary>
public class GenyouYakou() : KahoCard(0, CardType.Skill, CardRarity.Common, TargetType.None) {
  private const int HEARTS_THRESHOLD = 6;

  public override IEnumerable<CardKeyword> CanonicalKeywords => [
    CardKeyword.Ethereal,
  ];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new EnergyVar(1),
    new CardsVar(2),
  ];

  protected override bool ShouldGlowGoldInternal => HeartsState.GetHearts(Owner) <= HEARTS_THRESHOLD;

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    if (!ShouldGlowGoldInternal) return;
    await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    await CommonActions.Draw(this, ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.Cards.UpgradeValueBy(1m);
  }
}
