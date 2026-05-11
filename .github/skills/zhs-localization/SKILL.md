---
name: zhs-localization
description: Rules and conventions for writing and reviewing Chinese (ZHS) localization strings in LinkuraMod. Covers keyword wrapping, energy icon syntax, description vs smartDescription patterns, and common mistakes to avoid.
---

# ZHS Localization Guide for LinkuraMod

Reference this skill when creating or reviewing ZHS localization strings in
`LinkuraMod/localization/zhs/`. Always compare against the official STS2 ZHS
strings at `../sts2/localization/zhs/` as the authoritative source.

---

## File Structure

| File                     | Purpose                                                                                                                                                        |
| ------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `cards.json`             | Card titles + descriptions                                                                                                                                     |
| `powers.json`            | Power titles, `description`, `smartDescription`, `selectionScreenPrompt`                                                                                       |
| `relics.json`            | Relic titles, descriptions, flavor text                                                                                                                        |
| `card_keywords.json`     | Custom keyword titles + descriptions (does NOT include engine-native keywords like Exhaust, Retain, Ethereal — those are in the engine and need no entry here) |
| `static_hover_tips.json` | Custom hover-tip definitions (e.g. `爆心`)                                                                                                                     |
| `characters.json`        | Character-specific strings                                                                                                                                     |

---

## `[gold]` Tag Rules — Which Terms Must Be Wrapped

In card and power descriptions, **named game concepts that have hover-tip definitions must be wrapped in `[gold]...[/gold]`**. This makes them clickable in-game.

### Always Wrap

| Term   | Wrapped form          | Notes                                                                                                   |
| ------ | --------------------- | ------------------------------------------------------------------------------------------------------- |
| 格挡   | `[gold]格挡[/gold]`   | Block — appears after every `{Block:diff()}点`                                                          |
| 手牌   | `[gold]手牌[/gold]`   | Hand                                                                                                    |
| 弃牌堆 | `[gold]弃牌堆[/gold]` | Discard pile                                                                                            |
| 抽牌堆 | `[gold]抽牌堆[/gold]` | Draw pile                                                                                               |
| 牌组   | `[gold]牌组[/gold]`   | Deck                                                                                                    |
| 能量   | `[gold]能量[/gold]`   | Energy _as a word_ (only in plain `description` fields; use `energyIcons()` form in `smartDescription`) |
| 消耗   | `[gold]消耗[/gold]`   | Exhaust (verb, e.g. "将其消耗")                                                                         |
| 升级   | `[gold]升级[/gold]`   | Upgrade (verb)                                                                                          |
| 无实体 | `[gold]无实体[/gold]` | Intangible — official `INTANGIBLE_POWER.title`                                                          |

### Mod-Specific Keywords (Always Wrap)

| Term     | Wrapped form            |
| -------- | ----------------------- |
| 幕后     | `[gold]幕后[/gold]`     |
| 爆心     | `[gold]爆心[/gold]`     |
| 收心     | `[gold]收心[/gold]`     |
| 自然爆心 | `[gold]自然爆心[/gold]` |

### Do NOT Wrap

- Card type words standing alone: `技能牌`, `攻击牌`, `能力牌` — these are NOT individually hover-tipped in the official game
- Numbers: use `[blue]...[/blue]` instead (see below)
- Engine-native keywords on cards (Exhaust, Retain, Ethereal, Innate, etc.) — the engine's `AutoKeywordPosition` system appends them automatically; do NOT write them in descriptions

---

## Energy Icon Syntax

### In Card Descriptions

Use the card's `Energy` DynamicVar:

```
{Energy:energyIcons()}
```

Example: `"获得{Energy:energyIcons()}。"`

### In Power `smartDescription`

Use the power's own `Amount`:

```
{Amount:energyIcons()}
```

Example: `"在你的回合开始时，获得{Amount:energyIcons()}。"`

### In Power plain `description` (NO `{Amount}`)

Prefer official-style fixed energy icons when the value is constant (no `{Amount}` / no `Energy` DynamicVar):

```
{energyPrefix:energyIcons(1)}
```

Example:

```
"在你的回合开始时，获得{energyPrefix:energyIcons(1)}。"
"每当你的❤️上限变化，你在本回合打出的下一张牌耗能减少{energyPrefix:energyIcons(1)}。"
```

Fallback (also acceptable if icon-prefix form is not used) is to hardcode `[blue]N[/blue]点[gold]能量[/gold]`:

```
"在你的回合开始时，获得[blue]1[/blue]点[gold]能量[/gold]。"
```

Official precedent for the icon-prefix pattern: `FreePowerPower` uses `{energyPrefix:energyIcons(1)}` style formatting.

**Never use `{Energy:energyIcons()}` in a power description** if the power class has no `Energy` DynamicVar. Check the power's C# class to confirm which DynamicVars are available.

---

## `description` vs `smartDescription` in Powers

| Field              | Purpose                                                     | Variable access                                           |
| ------------------ | ----------------------------------------------------------- | --------------------------------------------------------- |
| `description`      | Shown when hovering the power icon (no dynamic info)        | **No `{Amount}` — hardcode the base number**              |
| `smartDescription` | Shown in the selection/upgrade screen with full live values | Full access to `{Amount}`, `{Amount:energyIcons()}`, etc. |

### Correct pattern for stackable count

```json
"description": "你每回合打出的前[blue]2[/blue]张[gold]幕后[/gold]牌耗能减少1。",
"smartDescription": "你每回合打出的前{Amount}张[gold]幕后[/gold]牌耗能减少1。"
```

- Base value in `description` must match the value passed to `PowerCmd.Apply(...)` in the card's `OnPlay`.
- `{Amount}` in `smartDescription` automatically reflects the live stacked count.

---

## Highlighting Numbers in Descriptions

Use `[blue]N[/blue]` for any hardcoded numeric values:

```
"在每回合开始时获得[blue]1[/blue]点[gold]能量[/gold]。"
"你每回合打出的前[blue]2[/blue]张[gold]幕后[/gold]牌耗能减少1。"
```

Dynamic values via `{Var:diff()}` are already highlighted by the engine — do not additionally wrap them in `[blue]`.

---

## Hearts Counter Unit

When referring to a count of ❤️, always use the counter word **颗**:

```
✅ "你每拥有6颗❤️"
✅ "每溢出20颗❤️"
✅ "每当你累计[gold]爆心[/gold]10颗"
❌ "你每拥有6❤️"
❌ "每溢出20❤️"
```

This mirrors the official STS2 ZHS pattern (e.g., `GENYOU_YAKOU.description`: `"如果你拥有8颗或更多❤️"`).

---

## Power Entry Required Fields

Every power must have all of these keys:

```json
"LINKURA_MOD_MY_POWER.title": "...",
"LINKURA_MOD_MY_POWER.description": "...",
"LINKURA_MOD_MY_POWER.smartDescription": "...",
```

---

## Localization Key Format

```
LINKURA_MOD_{UPPER_SNAKE_CLASS_NAME}.{field}
```

- Class name → `UPPER_SNAKE_CASE` (e.g. `EnduringTraditionPower` → `ENDURING_TRADITION_POWER`)
- Fields: `title`, `description`, `smartDescription`, `selectionScreenPrompt`
- Card keyword / hover-tip keys follow the same pattern

---

## Common Mistakes Found in Review

| Mistake                                                           | Correct                                          |
| ----------------------------------------------------------------- | ------------------------------------------------ |
| Untranslated English in ZHS file (e.g. `[gold]Auto Burst[/gold]`) | `[gold]自然爆心[/gold]`                          |
| Wrong keyword: `[gold]无形[/gold]` for Intangible                 | `[gold]无实体[/gold]` (`INTANGIBLE_POWER.title`) |
| Wrong keyword: `[gold]后台[/gold]` for Backstage                  | `[gold]幕后[/gold]`                              |
| Plain description uses `{Amount}`                                 | Hardcode the base value as `[blue]N[/blue]`      |
| Plain description says `获得能量` with no icon or number          | `获得[blue]1[/blue]点[gold]能量[/gold]`          |
| Plain description says `抽牌` without a count                     | `抽1张牌`                                        |
| `格挡`, `手牌`, `弃牌堆`, `抽牌堆` not wrapped in `[gold]`        | Always wrap these terms                          |
| `手牌少于X张` inside a card description — `手牌` not gold         | `[gold]手牌[/gold]少于X张`                       |
| ❤️ counts missing 颗 (e.g. "6❤️", "20❤️")                         | "6颗❤️", "20颗❤️"                                |
| Vague `description` like "前几张" or "额外触发"                   | "前[blue]N[/blue]张" / "额外触发一次"            |

---

## Workflow for New Cards/Powers

1. **Determine the class name** → convert to `UPPER_SNAKE_CASE` for the key prefix.
2. **Read the C# implementation** to confirm:
   - Which DynamicVars are declared (determines which `{Var:diff()}` tokens you can use).
   - Base values passed to power stacks (for hardcoding in `description`).
   - Actual mechanic (e.g., Collect once vs N times) for accurate phrasing.
3. **Write `description`** using only hardcoded values and `[gold]`-tagged keywords.
4. **Write `smartDescription`** using `{Amount}` / `{Amount:energyIcons()}` where applicable.
5. **Wrap all game terms** listed in the table above.
6. **Use颗** for all ❤️ count references.
7. **Keep parallel with ENG** — every key in ZHS must have a matching key in ENG.
