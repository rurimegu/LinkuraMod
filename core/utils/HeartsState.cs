using System;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Powers;

namespace RuriMegu.Core.Utils;

public static class HeartsState {
  public const int DEFAULT_MAX_HEARTS = 9;
  public const int MAX_MAX_HEARTS = 9999;

  public static int GetHearts(Player player) => GetAmount<HeartsPower>(player);

  public static int GetMaxHearts(Player player) {
    int maxHearts = GetAmount<MaxHeartsPower>(player);
    return maxHearts <= 0 ? DEFAULT_MAX_HEARTS : maxHearts;
  }

  public static async Task Reset(Player player, PlayerChoiceContext ctx) {
    await SetMaxHearts(player, ctx, DEFAULT_MAX_HEARTS);
    await SetHearts(player, ctx, 0);
  }

  public static Task<Events.HeartsChangedEvent> AddHearts(Player player, PlayerChoiceContext ctx, int amount, CardModel source = null) {
    return SetHearts(player, ctx, GetHearts(player) + amount, source);
  }

  public static Task<Events.MaxHeartsChangedEvent> AddMaxHearts(Player player, PlayerChoiceContext ctx, int amount, CardModel source = null) {
    return SetMaxHearts(player, ctx, GetMaxHearts(player) + amount, source);
  }

  public static async Task<Events.HeartsChangedEvent> SetHearts(Player player, PlayerChoiceContext ctx, int amount, CardModel source = null) {
    int oldHearts = GetHearts(player);
    int clampedAmount = Math.Clamp(amount, 0, GetMaxHearts(player));
    if (oldHearts == clampedAmount) {
      return null;
    }

    var ev = new Events.HeartsChangedEvent(
      player,
      ctx,
      oldHearts,
      clampedAmount,
      GetMaxHearts(player),
      clampedAmount - oldHearts,
      source
    );

    if (!await Events.HeartsChanged.InvokeAllEarly(ev)) return ev;

    await SetAmount<HeartsPower>(player, clampedAmount, source);

    await Events.HeartsChanged.InvokeAllLate(ev);
    return ev;
  }

  public static async Task<Events.MaxHeartsChangedEvent> SetMaxHearts(Player player, PlayerChoiceContext ctx, int amount, CardModel source = null) {
    int clampedAmount = Math.Clamp(amount, 0, MAX_MAX_HEARTS);
    int oldMaxHearts = GetMaxHearts(player);
    if (clampedAmount == oldMaxHearts) return null;

    var ev = new Events.MaxHeartsChangedEvent(
      player,
      ctx,
      oldMaxHearts,
      clampedAmount,
      GetHearts(player),
      clampedAmount - oldMaxHearts,
      source
    );

    if (!await Events.MaxHeartsChanged.InvokeAllEarly(ev)) return ev;

    await SetAmount<MaxHeartsPower>(player, clampedAmount, source);

    if (GetHearts(player) > clampedAmount) {
      await SetHearts(player, ctx, clampedAmount, source);
    }

    await Events.MaxHeartsChanged.InvokeAllLate(ev);
    return ev;
  }

  private static int GetAmount<TPower>(Player player) where TPower : PowerModel {
    return (int)(player.Creature.Powers.OfType<TPower>().FirstOrDefault()?.Amount ?? 0m);
  }

  private static Task<TPower> SetAmount<TPower>(Player player, int amount, CardModel source = null) where TPower : PowerModel {
    return PowerCmd.SetAmount<TPower>(player.Creature, amount, player.Creature, source);
  }
}
