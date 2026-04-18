using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Dream Believers — Cost 1 (0), Skill, Uncommon.
/// For every 6 ❤️ you have, gain {Energy:energyIcons()}. Exhaust.
/// </summary>
public class DreamBelievers() : KahoCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  private const int HEARTS_PER_ENERGY = 6;

  public override IEnumerable<CardKeyword> CanonicalKeywords => [
    CardKeyword.Exhaust,
  ];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new EnergyVar(1),
  ];

  protected override bool ShouldGlowGoldInternal => HeartsState.GetHearts(Owner) >= HEARTS_PER_ENERGY;

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int hearts = HeartsState.GetHearts(Owner);
    int energyGain = hearts / HEARTS_PER_ENERGY;
    if (energyGain > 0) {
      await PlayerCmd.GainEnergy(energyGain * DynamicVars.Energy.IntValue, Owner);
    }
  }

  protected override void OnUpgrade() {
    RemoveKeyword(CardKeyword.Exhaust);
  }
}
