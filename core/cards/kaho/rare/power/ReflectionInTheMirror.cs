using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Powers.Kaho;

namespace RuriMegu.Core.Cards.Kaho.Rare.Power;

/// <summary>
/// Reflection in the Mirror (闀滀腑鍊掑奖) 鈥?Cost 1 (0), Power, Rare.
/// Effects that increase Max 鉂わ笍 are doubled.
/// </summary>
public class ReflectionInTheMirror() : KahoCard(1, CardType.Power, CardRarity.Rare, TargetType.None) {
  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await PowerCmd.Apply<ReflectionInTheMirrorPower>(Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() {
    EnergyCost.UpgradeBy(-1);
  }
}
