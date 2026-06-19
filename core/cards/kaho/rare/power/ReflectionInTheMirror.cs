using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using RuriMegu.Core.Powers.Kaho;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Rare.Power;

/// <summary>
/// Reflection in the Mirror (镜中倒影) — Cost 1 (0), Power, Rare.
/// Effects that increase Max ❤️ are doubled.
/// </summary>
public class ReflectionInTheMirror() : KahoCard(1, CardType.Power, CardRarity.Rare, TargetType.None) {
  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await Owner.PlayCastAnim();
    await PowerCmd.Apply<ReflectionInTheMirrorPower>(ctx, Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    EnergyCost.UpgradeBy(-1);
  }
}
