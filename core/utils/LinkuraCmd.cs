using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace RuriMegu.Core.Utils;

public static class LinkuraCmd {

  public static Task IncreaseMaxHearts(Player player, int amount, CardModel source = null) {
    return HeartsState.AddMaxHearts(player, amount, source);
  }

  public static Task BurstHearts(Player player, int amount, CardModel source = null) {
    return HeartsState.AddHearts(player, amount, source);
  }

  public static async Task CollectHearts(Player player, PlayerChoiceContext context, CardModel source = null, Creature target = null) {
    int hearts = HeartsState.GetHearts(player);
    await ApplyHeartDamage(hearts, target, player, context);
    await HeartsState.SetHearts(player, 0, source);
  }

  private static async Task<IEnumerable<Creature>> ApplyHeartDamage(decimal value, Creature target, Player player, PlayerChoiceContext choiceContext) {
    List<Creature> list = [.. from e in player.Creature.CombatState.GetOpponentsOf(player.Creature)
                           where e.IsHittable
                           select e];
    if (list.Count == 0) {
      return [];
    }
    IReadOnlyList<Creature> targets = (target == null) ? [player.RunState.Rng.CombatTargets.NextItem(list)] : [target];
    await CreatureCmd.Damage(choiceContext, targets, value, ValueProp.Unpowered, player.Creature);
    return targets;
  }
}
