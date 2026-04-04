using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace RuriMegu.Core.Utils;

public static class Events {
  public record Event {
    public bool IsCancelled { get; private set; } = false;
    public void Cancel() => IsCancelled = true;
  }

  public record HeartsChangedEvent(
    Player Player,
    PlayerChoiceContext Context,
    int OldHearts,
    int NewHearts,
    int MaxHearts,
    int Delta,
    CardModel Source
  ) : Event;

  public record MaxHeartsChangedEvent(
    Player Player,
    PlayerChoiceContext Context,
    int OldMaxHearts,
    int NewMaxHearts,
    int Hearts,
    int Delta,
    CardModel Source
  ) : Event;

  public record BurstEvent(
    Player Player,
    PlayerChoiceContext Context,
    int RequestedAmount,
    CardModel Source
  ) : Event {
    public int ActualAmount { get; set; } = 0;
    public HeartsChangedEvent HeartsChangedEvent { get; set; } = null;
  }

  public record AutoBurstEvent(
    Player Player,
    PlayerChoiceContext Context,
    int BaseAmount,
    CardModel Source
  ) : Event {
    public BurstEvent BurstEvent { get; set; } = null;
  }

  public record CollectEvent(
    Player Player,
    PlayerChoiceContext Context,
    CardModel Source
  ) : Event {
    public int Amount { get; set; } = 0;
    public IReadOnlyList<Creature> Targets { get; set; } = null;
    public bool DamageAllEnemies { get; set; } = false;
  }

  public record IncreaseMaxHeartsEvent(
    Player Player,
    PlayerChoiceContext Context,
    int RequestedAmount,
    CardModel Source
  ) : Event {
    public int ActualAmount { get; set; } = 0;
  }

  public record TriggerBackstageEvent(
    Player Player,
    PlayerChoiceContext Context,
    CardModel Source
  ) : Event {
    public int RepeatCount { get; set; } = 0;
  }

  public class PhasedEvent<TEvent> where TEvent : Event {
    public event Func<TEvent, Task> VeryEarly;
    public event Func<TEvent, Task> Early;
    public event Func<TEvent, Task> Late;
    public event Func<TEvent, Task> VeryLate;

    public Subscription SubscribeVeryEarly(Func<TEvent, Task> handler) {
      VeryEarly += handler;
      return new Subscription(() => VeryEarly -= handler);
    }

    public Subscription SubscribeEarly(Func<TEvent, Task> handler) {
      Early += handler;
      return new Subscription(() => Early -= handler);
    }

    public Subscription SubscribeLate(Func<TEvent, Task> handler) {
      Late += handler;
      return new Subscription(() => Late -= handler);
    }

    public Subscription SubscribeVeryLate(Func<TEvent, Task> handler) {
      VeryLate += handler;
      return new Subscription(() => VeryLate -= handler);
    }

    /// <summary>
    /// Returns true if the event was not cancelled.
    /// </summary>
    public async Task<bool> InvokeVeryEarly(TEvent e) {
      if (VeryEarly != null) {
        foreach (Func<TEvent, Task> handler in VeryEarly.GetInvocationList()) {
          await handler(e);
        }
      }
      return !e.IsCancelled;
    }

    /// <summary>
    /// Returns true if the event was not cancelled.
    /// </summary>
    public async Task<bool> InvokeEarly(TEvent e) {
      if (Early != null) {
        foreach (Func<TEvent, Task> handler in Early.GetInvocationList()) {
          await handler(e);
        }
      }
      return !e.IsCancelled;
    }

    /// <summary>
    /// Returns true if the event was not cancelled.
    /// </summary>
    public async Task<bool> InvokeAllEarly(TEvent e) {
      if (!await InvokeVeryEarly(e)) return false;
      if (!await InvokeEarly(e)) return false;
      return true;
    }

    public async Task InvokeLate(TEvent e) {
      if (Late != null) {
        foreach (var handler in Late.GetInvocationList().Cast<Func<TEvent, Task>>()) {
          await handler(e);
        }
      }
    }

    public async Task InvokeVeryLate(TEvent e) {
      if (VeryLate != null) {
        foreach (var handler in VeryLate.GetInvocationList().Cast<Func<TEvent, Task>>()) {
          await handler(e);
        }
      }
    }

    public async Task InvokeAllLate(TEvent e) {
      await InvokeLate(e);
      await InvokeVeryLate(e);
    }
  }

  public static readonly PhasedEvent<HeartsChangedEvent> HeartsChanged = new();
  public static readonly PhasedEvent<MaxHeartsChangedEvent> MaxHeartsChanged = new();
  public static readonly PhasedEvent<BurstEvent> Burst = new();
  public static readonly PhasedEvent<AutoBurstEvent> AutoBurst = new();
  public static readonly PhasedEvent<CollectEvent> Collect = new();
  public static readonly PhasedEvent<IncreaseMaxHeartsEvent> IncreaseMaxHearts = new();
  public static readonly PhasedEvent<TriggerBackstageEvent> TriggerBackstage = new();
}

public static class EventExtensions {
  public static bool IsNullOrCancelled<T>(this T ev) where T : Events.Event {
    return ev == null || ev.IsCancelled;
  }
}
