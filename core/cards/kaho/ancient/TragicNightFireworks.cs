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
/// Set your Max ❤️ to 99, but you can no longer increase Max ❤️.
/// At the end of combat, increase your Max HP based on the highest single increase to your Max ❤️.
/// </summary>
public class TragicNightFireworks() : KahoCard(2, CardType.Power, CardRarity.Ancient, TargetType.None) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new RepeatVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await Owner.PlayCastAnim();
    await HeartsState.SetMaxHearts(Owner, ctx, 99, this);
    await PowerCmd.Apply<TragicNightFireworksPower>(Owner.Creature, DynamicVars.Repeat.IntValue, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    DynamicVars.Repeat.UpgradeValueBy(1m);
  }
}
