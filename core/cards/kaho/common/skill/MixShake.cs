using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Mix shake!! 鈥?Cost 1, Skill, Common.
/// Gain 5 (8) block. Triggers once more for every 6 鈾?you have.
/// </summary>
public class MixShake() : KahoCard(1, CardType.Skill, CardRarity.Common, TargetType.None) {
  private const int HEARTS_PER_EXTRA_TRIGGER = 6;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BlockVar(5, ValueProp.Move),
  ];

  protected override bool ShouldGlowGoldInternal => HeartsState.GetHearts(Owner) >= HEARTS_PER_EXTRA_TRIGGER;

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int hearts = HeartsState.GetHearts(Owner);
    int triggers = 1 + hearts / HEARTS_PER_EXTRA_TRIGGER;
    for (int i = 0; i < triggers; i++) {
      await CommonActions.CardBlock(this, play);
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.Block.UpgradeValueBy(3m);
  }
}
