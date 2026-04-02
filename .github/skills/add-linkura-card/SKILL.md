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
    DynamicVars.Damage().UpgradeValueBy(3m);
  }
}
```

Use `LinkuraCard` unless the card specifically needs in-hand trigger behavior, in which case start from `InHandTriggerCard`.

### 2. Register the card in the pool

Update `GenerateAllCards()` in `core/characters/HinoshitaKahoCardPool.cs`:

```csharp
ModelDb.Card<MyCard>(),
```

### 3. Add localization in both languages

Update both language files when adding any card.

`linkuramod/localization/eng/cards.json`

```json
"RURIMEGU-MY_CARD.title": "My Card",
"RURIMEGU-MY_CARD.description": "Deal {Damage:diff()} damage."
```

`linkuramod/localization/zhs/cards.json`

```json
"RURIMEGU-MY_CARD.title": "我的牌",
"RURIMEGU-MY_CARD.description": "造成{Damage:diff()}点伤害。"
```

Localization key rule: convert the C# class name to `UPPER_SNAKE_CASE` and use `RURIMEGU-{CLASS_NAME}.{field}`.

Note that most of the built-in keywords, and the keywords annotated with `AutoKeywordPosition.Before` or `AutoKeywordPosition.After` will be automatically added to the card description, so no need to add them to i18n files again.

### 4. Add placeholder portrait assets

Copy the placeholder portrait files so the new filenames match the card class name:

- `linkuramod/images/card_portraits/kaho_defend.png` -> `linkuramod/images/card_portraits/my_card.png`
- `linkuramod/images/card_portraits/big/kaho_defend.png` -> `linkuramod/images/card_portraits/big/my_card.png`

If custom art already exists, use that instead of the placeholder copies.

## Completion Checks

- The card class is in the correct folder and namespace.
- The selected base class matches the intended behavior.
- The card is registered in `HinoshitaKahoCardPool`.
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
