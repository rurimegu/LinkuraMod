using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// Half-Conveyed Feelings (传达一半的心意) — Cost 1, Skill, Uncommon.
/// Burst Hearts equal to half your max ❤️.
/// </summary>
public class HalfConveyedFeelings() : LinkuraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int maxHearts = HeartsState.GetMaxHearts(Owner);
    int burstAmount = maxHearts / 2;
    await LinkuraCmd.BurstHearts(Owner, burstAmount, this);
  }

  protected override void OnUpgrade() {
    // No upgrade change needed as the effect is based on current max hearts
  }
}
