# LinkuraMod — Agent Instructions

STS2 mod built with the **BaseLib** framework (`Alchyr.Sts2.BaseLib`) on Godot 4 / .NET 9.

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
LinkuraMod/
  localization/    # JSON string tables — eng/ and zhs/
  scenes/          # .tscn scene files
```

Root namespace: **`RuriMegu`**  
Sub-namespaces follow the folder path, e.g. `RuriMegu.Core.Cards.Kaho`.

---

## Naming Conventions

| What                                              | Convention                  | Example                                       |
| ------------------------------------------------- | --------------------------- | --------------------------------------------- |
| Constants (`const`)                               | `UPPER_SNAKE_CASE`          | `MAX_TRIGGERS_PER_PLAY`, `DEFAULT_MAX_HEARTS` |
| Public identifiers (classes, properties, methods) | `PascalCase`                | `BurstHearts()`, `CanonicalVars`              |
| Private fields                                    | `_camelCase`                | `_triggerCount`                               |
| Localization keys                                 | `RURIMEGU-CLASS_NAME.field` | `RURIMEGU-SPECIAL_THANKS.title`               |
| CustomEnum strings (keywords/DynamicVars)         | `PascalCase_Words`          | `"Collect_Hearts"`, `"Backstage"`             |

> **Do NOT use PascalCase for constants.** Always `UPPER_SNAKE_CASE`.

Card creation workflow lives in `.github/skills/add-linkura-card/SKILL.md`.

Potion creation workflow lives in `.github/skills/add-linkura-potion/SKILL.md`.

---

## Card Base Classes

| Class               | Use When                                                                          |
| ------------------- | --------------------------------------------------------------------------------- |
| `LinkuraCard`       | Standard card                                                                     |
| `InHandTriggerCard` | Card with a **Backstage** effect — triggers while in hand when a condition is met |

## Localization

- Two languages always maintained in parallel: **`eng`** and **`zhs`**.
- Key format: `RURIMEGU-{UPPER_SNAKE_CLASS_NAME}.{field}` where field is `title` or `description`.
- Class name in the key is the C# class name converted to `UPPER_SNAKE_CASE`.
- In-game rich text tags: `[gold]Keyword Name[/gold]` for highlighted keyword references in descriptions.
- Variable references in descriptions: `{VariableName:diff()}`.

---

## Decompiled Source

The decompiled source code of the original game is located in the `../sts2` folder. You can use it as a reference for the original game's implementation.

---

## Code Style

- All gameplay actions are `async Task`. Always `await` them; never fire-and-forget.
- Prefer guard clauses (`if (...) return;`) over nesting in hook overrides.
- Put duplicate code in the `utils` folder. Do not repeat the same logic in different places.
- This project does **not** use `#nullable` annotations. Do not write nullable types (`string?`, `Creature?`, etc.) — use non-nullable types throughout.
- Do not edit / output to `.uid` or `.import` files - they are automatically generated. Only create such files with care if you know exactly why they are needed and what they do - for example, when creating new assets depending on each other, so uids must be known to reference.
