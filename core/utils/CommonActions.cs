using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace RuriMegu.Core.Utils;

/// <summary>
/// Utility class providing common gameplay actions using native game APIs.
/// </summary>
public static class CommonActions {
  private const string DEFAULT_HIT_FX = "vfx/vfx_attack_slash";

  /// <summary>
  /// Builds a card attack command using the card's Damage DynamicVar and CardPlay target.
  /// Call .Execute(ctx) on the result to execute.
  /// </summary>
  public static AttackCommand CardAttack(CardModel card, CardPlay play, int hitCount = 1) {
    var cmd = DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
      .FromCard(card)
      .WithHitFx(DEFAULT_HIT_FX);
    if (play.Target != null)
      cmd = cmd.Targeting(play.Target);
    if (hitCount > 1)
      cmd = cmd.WithHitCount(hitCount);
    return cmd;
  }

  /// <summary>
  /// Builds a card attack command targeting a specific creature, using the card's Damage DynamicVar.
  /// Call .Execute(ctx) on the result to execute.
  /// </summary>
  public static AttackCommand CardAttack(CardModel card, Creature target, int hitCount = 1) {
    var cmd = DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
      .FromCard(card)
      .WithHitFx(DEFAULT_HIT_FX);
    if (target != null)
      cmd = cmd.Targeting(target);
    if (hitCount > 1)
      cmd = cmd.WithHitCount(hitCount);
    return cmd;
  }

  /// <summary>
  /// Builds a card attack command with a custom damage amount targeting a specific creature.
  /// Call .Execute(ctx) on the result to execute.
  /// </summary>
  public static AttackCommand CardAttack(CardModel card, Creature target, decimal damage, int hitCount = 1) {
    var cmd = DamageCmd.Attack(damage)
      .FromCard(card)
      .WithHitFx(DEFAULT_HIT_FX);
    if (target != null)
      cmd = cmd.Targeting(target);
    if (hitCount > 1)
      cmd = cmd.WithHitCount(hitCount);
    return cmd;
  }

  /// <summary>
  /// Builds a card attack command using a CalculatedDamageVar targeting a specific creature.
  /// Call .Execute(ctx) on the result to execute.
  /// </summary>
  public static AttackCommand CardAttack(CardModel card, Creature target, CalculatedDamageVar calculatedDamage, int hitCount = 1) {
    var cmd = DamageCmd.Attack(calculatedDamage)
      .FromCard(card)
      .WithHitFx(DEFAULT_HIT_FX);
    if (target != null)
      cmd = cmd.Targeting(target);
    if (hitCount > 1)
      cmd = cmd.WithHitCount(hitCount);
    return cmd;
  }

  /// <summary>
  /// Builds a card attack command using the card's Damage DynamicVar targeting all opponents.
  /// Call .Execute(ctx) on the result to execute.
  /// </summary>
  public static AttackCommand CardAttackAllOpponents(CardModel card, int hitCount = 1) {
    var cmd = DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
      .FromCard(card)
      .WithHitFx(DEFAULT_HIT_FX)
      .TargetingAllOpponents(card.Owner.Creature.CombatState);
    if (hitCount > 1)
      cmd = cmd.WithHitCount(hitCount);
    return cmd;
  }

  /// <summary>
  /// Builds a card attack command with a custom damage amount targeting all opponents.
  /// Call .Execute(ctx) on the result to execute.
  /// </summary>
  public static AttackCommand CardAttackAllOpponents(CardModel card, decimal damage, int hitCount = 1) {
    var cmd = DamageCmd.Attack(damage)
      .FromCard(card)
      .WithHitFx(DEFAULT_HIT_FX)
      .TargetingAllOpponents(card.Owner.Creature.CombatState);
    if (hitCount > 1)
      cmd = cmd.WithHitCount(hitCount);
    return cmd;
  }

  /// <summary>
  /// Builds a card attack command using a CalculatedDamageVar targeting all opponents.
  /// Call .Execute(ctx) on the result to execute.
  /// </summary>
  public static AttackCommand CardAttackAllOpponents(CardModel card, CalculatedDamageVar calculatedDamage, int hitCount = 1) {
    var cmd = DamageCmd.Attack(calculatedDamage)
      .FromCard(card)
      .WithHitFx(DEFAULT_HIT_FX)
      .TargetingAllOpponents(card.Owner.Creature.CombatState);
    if (hitCount > 1)
      cmd = cmd.WithHitCount(hitCount);
    return cmd;
  }

  /// <summary>
  /// Gains block using the card's Block DynamicVar.
  /// </summary>
  public static Task CardBlock(CardModel card, CardPlay play) {
    return CreatureCmd.GainBlock(card.Owner.Creature, card.DynamicVars.Block, play);
  }

  /// <summary>
  /// Gains block using a specific BlockVar.
  /// </summary>
  public static Task CardBlock(CardModel card, BlockVar blockVar, CardPlay play) {
    return CreatureCmd.GainBlock(card.Owner.Creature, blockVar, play);
  }

  /// <summary>
  /// Draws cards equal to the card's Cards DynamicVar value.
  /// Returns the drawn cards.
  /// </summary>
  public static Task<IEnumerable<CardModel>> Draw(CardModel card, PlayerChoiceContext ctx) {
    return CardPileCmd.Draw(ctx, card.DynamicVars.Cards.IntValue, card.Owner);
  }
}
