---
name: add-linkura-potion
description: "Create a new LinkuraMod potion. Use when adding or updating a potion."
argument-hint: "Provide the potion name, target character, rarity (Common/Uncommon/Rare), and the effect description."
---

# Add Linkura Potion

## When to Use

- Add a new potion to LinkuraMod.
- Register a potion in the Hinoshita Kaho potion pool.
- Update required localization for both supported languages.
- Add placeholder image assets for the potion.

## Inputs To Confirm

- Potion class name in PascalCase.
- Target character (currently only Kaho supported).
- Rarity: `Common`, `Uncommon`, or `Rare`.
- Combat-only or usable any time (`CombatOnly` / `AnyTime`).
- Whether the effect needs a backing Power class.

## Project Layout for Potions

```
core/
  potions/
    LinkuraPotion.cs       # abstract base for all mod potions
    kaho/
      KahoPotion.cs        # Kaho-specific base ([Pool] + CharacterId)
      common/              # Common rarity
      uncommon/            # Uncommon rarity
      rare/                # Rare rarity
LinkuraMod/
  images/
    potions/
      kaho/                # Potion images: <potion_name>.png + <potion_name>_outline.png
  localization/
    eng/potions.json
    zhs/potions.json
```

## Procedure

### 1. Create the potion class

Place in `core/potions/kaho/<rarity>/MyPotion.cs`, namespace `RuriMegu.Core.Potions.Kaho.<Rarity>`.

```csharp
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace RuriMegu.Core.Potions.Kaho.Common;

public class MyPotion : KahoPotion {
  public override PotionRarity Rarity => PotionRarity.Common;
  public override PotionUsage Usage => PotionUsage.CombatOnly;
  public override TargetType TargetType => TargetType.None;

  protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature target) {
    // implement effect
  }
}
```

Key APIs:

- `Owner` — the `Player` using the potion.
- `Owner.Creature` — the `Creature` of the owning player.
- `Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)` — the full character card pool (for picking cards by keyword, etc.).
- `CardFactory.GetDistinctForCombat(Owner, cards, count, Owner.RunState.Rng.CombatCardGeneration)` — pick N distinct cards with combat RNG.
- `CardSelectCmd.FromChooseACardScreen(ctx, cards, Owner, canSkip: true)` — show a card selection screen.
- `CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true)` — add a freshly generated card to hand.
- `card.GiveSingleTurnRetain()` — mark a card to be retained until end of turn (not discarded at turn end).
- `card.SetToFreeThisTurn()` — make the card cost 0 this turn.
- `LinkuraCmd.GainAutoBurst(Owner.Creature, amount, Owner.Creature, null)` — grant Auto Burst stacks.
- `PowerCmd.Apply<MyPower>(Owner.Creature, amount, Owner.Creature, null)` — apply a power.

### 2. (If needed) Create a backing Power

If the potion applies a per-turn buff, create a `core/powers/kaho/MyPotionPower.cs` following the power pattern.

For a "this turn only" power, subscribe to events in `AfterApplied` and remove at `AfterTurnEnd`:

```csharp
public override async Task AfterApplied(Creature applier, CardModel cardSource) {
  DisposeTrackedSubscriptions();
  TrackSubscription(Events.SomeEvent.SubscribeLate(OnSomeEvent));
  await base.AfterApplied(applier, cardSource);
}

public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side) {
  await base.AfterTurnEnd(choiceContext, side);
  if (side == Owner.Side) {
    await PowerCmd.Remove(this);
  }
}
```

Add localization for the power in both `eng/powers.json` and `zhs/powers.json`.

### 3. Add localization in both languages

Update `LinkuraMod/localization/eng/potions.json` and `zhs/potions.json`.

```json
"RURIMEGU-MY_POTION.title": "My Potion",
"RURIMEGU-MY_POTION.description": "Do something cool."
```

For potions that show a card selection screen, also add a `selectionScreenPrompt` key:

```json
"RURIMEGU-MY_POTION.selectionScreenPrompt": "Choose a card to add to your hand."
```

Localization key rule: convert the C# class name to `UPPER_SNAKE_CASE` and prefix with `RURIMEGU-`.

### 4. Add placeholder image assets

Place in `LinkuraMod/images/potions/kaho/`:

- `my_potion.png` — main potion image (displayed in the potion slot)
- `my_potion_outline.png` — outline version (used for hover/tooltip outlines)

The filename is the C# class name converted to `lower_snake_case`, with the `RURIMEGU-` model ID prefix stripped. The image paths are handled automatically by `KahoPotion` via `LinkuraPotion.CustomPackedImagePath`.

If no custom art is available yet, copy a placeholder from an existing potion in the same folder.

## Completion Checks

- Potion class is in the correct folder and namespace.
- `Rarity`, `Usage`, and `TargetType` are all overridden.
- Both `eng` and `zhs` localization entries exist in `potions.json`.
- If a backing power was added, its localization entries exist in `powers.json`.
- Image files `<potion_name>.png` and `<potion_name>_outline.png` exist in `LinkuraMod/images/potions/kaho/`.
- Run the debug build to confirm the mod still compiles.

## Repository Rules

- Root namespace is `RuriMegu`; sub-namespaces follow folder structure.
- Public identifiers use `PascalCase`; constants use `UPPER_SNAKE_CASE`; private fields use `_camelCase`.
- This project does **not** use `#nullable` annotations — do not write `string?`, `Creature?`, etc. Use non-nullable types throughout.
- Gameplay actions are `async Task` and must always be awaited.
- Prefer guard clauses over nested conditionals.
