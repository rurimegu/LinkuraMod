using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using RuriMegu.Core.Powers.Kaho;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Ancient;

/// <summary>
/// Tragic Night Fireworks (可惜夜花火) — Cost 2 (1), Power, Ancient.
/// Your max ❤️ is permanently fixed at 99. (Upgraded: also Innate.)
/// </summary>
public class TragicNightFireworks() : KahoCard(2, CardType.Power, CardRarity.Ancient, TargetType.None) {
  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await Owner.PlayCastAnim();
    await PowerCmd.Apply<TragicNightFireworksPower>(Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    EnergyCost.UpgradeBy(-1);
    AddKeyword(CardKeyword.Innate);
  }
}
