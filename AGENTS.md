# LinkuraMod — Agent Instructions

STS2 mod built with the **BaseLib** framework (`Alchyr.Sts2.BaseLib` v0.2.0) on Godot 4 / .NET 9.

---

## Build

```bash
# Debug (use this to verify changes compile)
dotnet build LinkuraMod.sln --configuration Debug

# Release
dotnet build LinkuraMod.sln --configuration ExportRelease
```

Always build after any C# change to confirm there are no errors.

---

## Project Layout

```
core/
  cards/           # Base card classes + shared DynamicVars
    kaho/          # Kaho-specific cards
  characters/      # Character model + card/relic/potion pools
  nodes/           # Godot scene backing nodes
  patches/         # Harmony / MonoMod patches
  powers/          # Power implementations
  relics/          # Relic implementations
  utils/           # Helpers (HeartsState, CardExtensions, etc.)
linkuramod/
  localization/    # JSON string tables — eng/ and zhs/
  scenes/          # .tscn scene files
```

Root namespace: **`RuriMegu`**  
Sub-namespaces follow the folder path, e.g. `RuriMegu.Core.Cards.Kaho`.

---

## Naming Conventions

| What | Convention | Example |
|---|---|---|
| Constants (`const`) | `UPPER_SNAKE_CASE` | `MAX_TRIGGERS_PER_PLAY`, `DEFAULT_MAX_HEARTS` |
| Public identifiers (classes, properties, methods) | `PascalCase` | `BurstHearts()`, `CanonicalVars` |
| Private fields | `_camelCase` | `_triggerCount` |
| Localization keys | `RURIMEGU-CLASS_NAME.field` | `RURIMEGU-SPECIAL_THANKS.title` |
| CustomEnum strings (keywords/DynamicVars) | `PascalCase_Words` | `"Collect_Hearts"`, `"Backstage"` |

> **Do NOT use PascalCase for constants.** Always `UPPER_SNAKE_CASE`.

---

## Adding a New Card

### 1. Create the card class

Place in `core/cards/kaho/` (or `core/cards/` for shared cards).

```csharp
namespace RuriMegu.Core.Cards.Kaho;

public class MyCard() : LinkuraCard(cost, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) {
  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(6, ValueProp.Move),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.Attack(this, ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage().UpgradeValueBy(3m);
  }
}
```

### 2. Register in the card pool

`core/characters/HinoshitaKahoCardPool.cs` → `GenerateAllCards()`:

```csharp
ModelDb.Card<MyCard>(),
```

### 3. Add localization

Both language files must be updated when adding any card.

`linkuramod/localization/eng/cards.json`:
```json
"RURIMEGU-MY_CARD.title": "My Card",
"RURIMEGU-MY_CARD.description": "Deal {Damage:diff()} damage."
```

`linkuramod/localization/zhs/cards.json`:
```json
"RURIMEGU-MY_CARD.title": "我的牌",
"RURIMEGU-MY_CARD.description": "造成{Damage:diff()}点伤害。"
```

---

## Card Base Classes

| Class | Use When |
|---|---|
| `LinkuraCard` | Standard card |
| `InHandTriggerCard` | Card with a **Backstage** effect — triggers while in hand when a condition is met |

## Localization

- Two languages always maintained in parallel: **`eng`** and **`zhs`**.
- Key format: `RURIMEGU-{UPPER_SNAKE_CLASS_NAME}.{field}` where field is `title` or `description`.
- Class name in the key is the C# class name converted to `UPPER_SNAKE_CASE`.
- In-game rich text tags: `[gold]Keyword Name[/gold]` for highlighted keyword references in descriptions.
- Variable references in descriptions: `{VariableName:diff()}`.

---

## Code Style

- All gameplay actions are `async Task`. Always `await` them; never fire-and-forget.
- Prefer guard clauses (`if (...) return;`) over nesting in hook overrides.
- Put duplicate code in the `utils` folder. Do not repeat the same logic in different places.
