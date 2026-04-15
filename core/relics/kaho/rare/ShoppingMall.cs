using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace RuriMegu.Core.Relics.Kaho.Rare;

/// <summary>
/// Shopping Mall — Rare relic for Hinoshita Kaho.
/// All enemies drop an extra card reward set.
/// At the end of each combat, lose 20 gold.
/// </summary>
public class ShoppingMall : KahoRelic {
  public override RelicRarity Rarity => RelicRarity.Rare;

  private const int GOLD_LOSS = 20;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new GoldVar(GOLD_LOSS),
  ];

  public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom room) {
    if (player != Owner) return false;
    if (room == null || !room.RoomType.IsCombatRoom()) return false;
    if (room.RoomType == RoomType.Boss && player.RunState.CurrentActIndex >= player.RunState.Acts.Count - 1) return false;
    rewards.Add(new CardReward(CardCreationOptions.ForRoom(player, room.RoomType), 3, player));
    return true;
  }

  public override Task AfterModifyingRewards() {
    Flash();
    return Task.CompletedTask;
  }

  public override async Task AfterCombatVictory(CombatRoom room) {
    await PlayerCmd.LoseGold(GOLD_LOSS, Owner, GoldLossType.Lost);
    await base.AfterCombatVictory(room);
  }
}
