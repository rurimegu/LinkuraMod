using System;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Powers;

namespace RuriMegu.Core.Utils;

public static class HeartsState {
  public readonly record struct HeartsChangedEvent(
    Player Player,
    int OldHearts,
    int NewHearts,
    int MaxHearts,
    int Delta,
    CardModel Source
  );

  public readonly record struct MaxHeartsChangedEvent(
    Player Player,
    int OldMaxHearts,
    int NewMaxHearts,
    int Hearts,
    int Delta,
    CardModel Source
  );

  public const int DEFAULT_MAX_HEARTS = 9;
  public const int MAX_MAX_HEARTS = 9999;

  public static event Action<HeartsChangedEvent> HeartsChanged;
  public static event Action<MaxHeartsChangedEvent> MaxHeartsChanged;

  public static int GetHearts(Player player) => GetAmount<HeartsPower>(player);

  public static int GetMaxHearts(Player player) {
    int maxHearts = GetAmount<MaxHeartsPower>(player);
    return maxHearts <= 0 ? DEFAULT_MAX_HEARTS : maxHearts;
  }

  public static async Task Reset(Player player) {
    await SetMaxHearts(player, DEFAULT_MAX_HEARTS);
    await SetHearts(player, 0);
  }

  public static Task AddHearts(Player player, int amount, CardModel source = null) {
    return SetHearts(player, GetHearts(player) + amount, source);
  }

  public static Task AddMaxHearts(Player player, int amount, CardModel source = null) {
    return SetMaxHearts(player, GetMaxHearts(player) + amount, source);
  }

  public static IDisposable SubscribeHeartsChanged(Action<HeartsChangedEvent> handler) {
    HeartsChanged += handler;
    return new Subscription(() => HeartsChanged -= handler);
  }

  public static IDisposable SubscribeMaxHeartsChanged(Action<MaxHeartsChangedEvent> handler) {
    MaxHeartsChanged += handler;
    return new Subscription(() => MaxHeartsChanged -= handler);
  }

  public static async Task SetHearts(Player player, int amount, CardModel source = null) {
    int oldHearts = GetHearts(player);
    int clampedAmount = Math.Clamp(amount, 0, GetMaxHearts(player));
    if (oldHearts == clampedAmount) {
      return;
    }

    await SetAmount<HeartsPower>(player, clampedAmount, source);

    HeartsChanged?.Invoke(new HeartsChangedEvent(
      player,
      oldHearts,
      clampedAmount,
      GetMaxHearts(player),
      clampedAmount - oldHearts,
      source
    ));
  }

  public static async Task SetMaxHearts(Player player, int amount, CardModel source = null) {
    int clampedAmount = Math.Clamp(amount, 0, MAX_MAX_HEARTS);
    int oldMaxHearts = GetMaxHearts(player);
    await SetAmount<MaxHeartsPower>(player, clampedAmount, source);

    if (GetHearts(player) > clampedAmount) {
      await SetHearts(player, clampedAmount, source);
    }
    
    MaxHeartsChanged?.Invoke(new MaxHeartsChangedEvent(
      player,
      oldMaxHearts,
      clampedAmount,
      GetHearts(player),
      clampedAmount - oldMaxHearts,
      source
    ));
  }

  private static int GetAmount<TPower>(Player player) where TPower : PowerModel {
    return (int)(player.Creature.Powers.OfType<TPower>().FirstOrDefault()?.Amount ?? 0m);
  }

  private static Task SetAmount<TPower>(Player player, int amount, CardModel source = null) where TPower : PowerModel {
    return PowerCmd.SetAmount<TPower>(player.Creature, amount, player.Creature, source);
  }
}
