---
name: relic
description: "Reference for implementing relics in LinkuraMod. Covers the LinkuraRelic base class lifecycle, subscription initialization pattern, mid-combat pickup, and persisting relic state across saves using SavedProperty."
---

# Relic Implementation Guide for LinkuraMod

---

## Base Classes

| Class                 | Use When                                   |
| --------------------- | ------------------------------------------ |
| `KahoRelic`           | Relic in the Hinoshita Kaho pool           |
| `LinkuraStarterRelic` | Starter relic (sets `Rarity = Starter`)    |
| `LinkuraRelic`        | Direct base (usually via one of the above) |

---

## Subscription Lifecycle — `InitializeSubscriptions`

`LinkuraRelic` mirrors the `LinkuraCard` subscription pattern.
Override `InitializeSubscriptions()` to register event subscriptions via `TrackSubscription()`.
**Do not** subscribe in `BeforeCombatStart` or `BeforeCombatStartLate`.

```csharp
protected override Task InitializeSubscriptions() {
    TrackSubscription(Events.Burst.SubscribeLate(OnBurstLate));
    return Task.CompletedTask;
}
```

The base class calls `InitializeSubscriptions` automatically:

- At `BeforeCombatStartLate` — for relics already in inventory at combat start.
- At `AfterObtained` when `CombatManager.Instance.IsInProgress` — for relics obtained mid-combat.

Subscriptions are disposed automatically at `AfterCombatEnd` and `AfterRemoved`.

### Do NOT call `TrackSubscription` in `BeforeCombatStart`

`BeforeCombatStart` fires before `InitializeSubscriptions`, and there is no
guard preventing double-initialization there. Use `BeforeCombatStart` only
for resetting non-subscription state (e.g. counters):

```csharp
public override Task BeforeCombatStart() {
    _counter = 0; // reset combat-local state
    return Task.CompletedTask;
}
```

---

## Persisting State Across Saves — `[SavedProperty]`

Relic fields that must survive save/load (e.g. cross-combat counters, accumulated values) must be exposed as **properties** decorated with `[SavedProperty]`.

```csharp
using MegaCrit.Sts2.Core.Saves.Runs;

private int _accumulatedOverflow;

[SavedProperty]
public int AccumulatedOverflow {
    get => _accumulatedOverflow;
    set {
        AssertMutable();
        _accumulatedOverflow = value;
        InvokeDisplayAmountChanged(); // if used in ShowCounter
    }
}
```

Rules:

- Must be a **property** (not a field) with a `set` accessor.
- Call `AssertMutable()` at the top of every setter.
- Use `SerializationCondition.SaveIfNotTypeDefault` to skip saving the default value and keep save data compact.
- When the property drives `DisplayAmount`, call `InvokeDisplayAmountChanged()` in the setter.
- `private` properties are supported (the serializer uses reflection).

### When to NOT use `[SavedProperty]`

- Combat-local counters that should reset every combat — reset in `BeforeCombatStart`, no attribute needed.
- Derived/computed values — derive them from saved properties instead.
- `CancellationTokenSource` or other transient runtime objects — never save these.

---

## `ShowCounter` / `DisplayAmount`

Override both to surface a counter badge on the relic icon:

```csharp
public override bool ShowCounter => true;
public override int DisplayAmount => AccumulatedOverflow;
```

If `DisplayAmount` is driven by a `[SavedProperty]`, call `InvokeDisplayAmountChanged()` in the setter so the UI updates live.

---

## `AfterObtained` Overrides

When overriding `AfterObtained` for pickup effects (e.g. `CapLiberator`), always call `await base.AfterObtained()` last. The base implementation handles mid-combat subscription initialization.

```csharp
public override async Task AfterObtained() {
    // your pickup logic here
    await base.AfterObtained(); // important: triggers InitializeSubscriptions if in combat
}
```
