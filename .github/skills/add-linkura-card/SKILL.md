---
name: add-linkura-card
description: "Create a new LinkuraMod card for Slay the Spire 2. Use when adding a new card, registering it in the character card pool, updating eng and zhs localization, and adding placeholder card portrait assets."
argument-hint: "Provide the card name, target character or shared scope, and whether it uses standard or Backstage behavior."
---

# Add Linkura Card

## When to Use

- Add a new playable card to LinkuraMod.
- Register a card in the Hinoshita Kaho card pool.
- Update required localization for both supported languages.
- Add placeholder portrait assets that match the card class name.

## Inputs To Confirm

- Card class name in PascalCase.
- Card scope: `core/cards/kaho/` for Kaho-specific cards, or `core/cards/` for shared cards.
- Card base class: `LinkuraCard` for standard cards, `InHandTriggerCard` for Backstage-style hand triggers.
- Cost, rarity, target type, and the DynamicVars the card needs.

## Procedure

### 1. Create the card class

Place the class in the correct folder and namespace.

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
    DynamicVars.Damage.UpgradeValueBy(3m);
  }
}
```

Use `LinkuraCard` unless the card specifically needs in-hand trigger behavior, in which case start from `InHandTriggerCard`.

### 2. Add localization in both languages

Update both language files when adding any card.

`LinkuraMod/localization/eng/cards.json`

```json
"LINKURA_MOD_CARD_MY_CARD.title": "My Card",
"LINKURA_MOD_CARD_MY_CARD.description": "Deal {Damage:diff()} damage."
```

`LinkuraMod/localization/zhs/cards.json`

```json
"LINKURA_MOD_CARD_MY_CARD.title": "我的牌",
"LINKURA_MOD_CARD_MY_CARD.description": "造成{Damage:diff()}点伤害。"
```

Localization key rule: RitsuLib derives the key as `LINKURA_MOD_CARD_{CLASS_NAME}.{field}` — ModId (`LINKURA_MOD`) + `CARD` category + the C# class name converted to `UPPER_SNAKE_CASE`.

Note that most of the built-in keywords, and the keywords annotated with `AutoKeywordPosition.Before` or `AutoKeywordPosition.After` will be automatically added to the card description, so no need to add them to i18n files again. Examples: `Ethereal`, `Exhaust`, `Retain`.

You should always create an `EnergyVar` for energy-related effects, and write `Energy:energyIcons()}` in the description to display the energy icons. Do not use raw text like "1 energy".

### 3. Add placeholder portrait assets

Copy the placeholder portrait files so the new filenames match the card class name:

- `LinkuraMod/images/card_portraits/kaho_defend.png` -> `LinkuraMod/images/card_portraits/my_card.png`
- `LinkuraMod/images/card_portraits/big/kaho_defend.png` -> `LinkuraMod/images/card_portraits/big/my_card.png`

If custom art already exists, use that instead of the placeholder copies.

## Completion Checks

- The card class is in the correct folder and namespace.
- The selected base class matches the intended behavior.
- The card extends `KahoCard` (which carries `[RegisterCard(typeof(HinoshitaKahoCardPool), Inherit = true)]`), so it is automatically registered in the Kaho card pool — no explicit registration step is needed.
- Both `eng` and `zhs` localization entries exist.
- Portrait filenames match the class name in snake_case form used by the mod assets.
- If C# files changed, run the debug build to confirm the mod still compiles.

## Dynamic Damage Rule (Important)

- For cards with changing damage (for example, in-hand reductions or scaling while in combat), treat `DynamicVars.Damage.BaseValue` as the source of truth.
- Do not calculate final damage only at play time (for example, `DamageCmd.Attack(baseDamage - penalty)` in `OnPlay`).
- Update and store the card's current damage value when state changes happen (trigger events, turn transitions, upgrade/downgrade recalculations).
- If the description uses a custom damage preview var, mirror it from the stored `DynamicVars.Damage.BaseValue` so display and execution always match.
- This avoids preview mismatches and ensures effects like Weak/Vulnerable use consistent displayed values.

## Repository Rules

- Root namespace is `RuriMegu` and sub-namespaces follow folder structure.
- Public identifiers use PascalCase; constants use `UPPER_SNAKE_CASE`; private fields use `_camelCase`.
- Gameplay actions are `async Task` and should always be awaited.
- Prefer guard clauses over nested conditionals.
- Extract duplicated logic into `core/utils/` rather than repeating it across cards.
