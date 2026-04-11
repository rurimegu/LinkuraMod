using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Dream Believers 鈥?Cost 1 (0), Skill, Uncommon.
/// For every 6 鉂わ笍 you have, gain {Energy:energyIcons()}. Collect. Exhaust.
/// </summary>
public class DreamBelievers() : KahoCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  private const int HEARTS_PER_ENERGY = 6;

  public override IEnumerable<CardKeyword> CanonicalKeywords => [
    LinkuraKeywords.Collect,
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
    await LinkuraCardActions.CollectHearts(this, ctx);
  }

  protected override void OnUpgrade() {
    RemoveKeyword(CardKeyword.Exhaust);
  }
}
