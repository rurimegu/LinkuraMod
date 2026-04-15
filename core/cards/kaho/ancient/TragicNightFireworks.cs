using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Powers.Kaho;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Ancient;

/// <summary>
/// Tragic Night Fireworks (可惜夜花火) — Cost 2, Power, Ancient.
/// Set Max ❤️ to 99. When you increase Max ❤️, raise your Max HP by 1 (2) instead.
/// </summary>
public class TragicNightFireworks() : KahoCard(2, CardType.Power, CardRarity.Ancient, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new MaxHpVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await Owner.PlayCastAnim();
    await HeartsState.SetMaxHearts(Owner, ctx, 99, this);
    await PowerCmd.Apply<TragicNightFireworksPower>(Owner.Creature, DynamicVars.MaxHp.IntValue, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.MaxHp.UpgradeValueBy(1);
  }
}
