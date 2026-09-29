using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace RuriMegu.Core.Utils;

/// <summary>
/// Bridges game API calls whose signatures differ between stable and beta.
/// </summary>
public static class GameApiCompat {
  private static readonly MethodInfo _setSpineAnimation = typeof(MegaAnimationState).GetMethod(
    nameof(MegaAnimationState.SetAnimation), [typeof(string), typeof(bool), typeof(int)])
    ?? throw new MissingMethodException(typeof(MegaAnimationState).FullName, nameof(MegaAnimationState.SetAnimation));
  private static readonly MethodInfo _addSpineAnimation = typeof(MegaAnimationState).GetMethod(
    nameof(MegaAnimationState.AddAnimation), [typeof(string), typeof(float), typeof(bool), typeof(int)])
    ?? throw new MissingMethodException(typeof(MegaAnimationState).FullName, nameof(MegaAnimationState.AddAnimation));
  private static readonly Func<AttackCommand, CardModel, CardPlay, AttackCommand> _fromCardWithPlay;
  private static readonly Func<AttackCommand, CardModel, AttackCommand> _fromCardWithoutPlay;
  private static readonly Func<PlayerChoiceContext, Creature, DamageVar, Creature, CardModel, CardPlay, Task<IEnumerable<DamageResult>>> _damageCardWithPlay;
  private static readonly Func<PlayerChoiceContext, Creature, DamageVar, Creature, CardModel, Task<IEnumerable<DamageResult>>> _damageCardWithoutPlay;
  private static readonly Func<PlayerChoiceContext, IEnumerable<Creature>, decimal, ValueProp, Creature, CardModel, CardPlay, Task<IEnumerable<DamageResult>>> _damageTargetsWithPlay;
  private static readonly Func<PlayerChoiceContext, IEnumerable<Creature>, decimal, ValueProp, Creature, CardModel, Task<IEnumerable<DamageResult>>> _damageTargetsWithoutPlay;

  static GameApiCompat() {
    Type commandType = typeof(AttackCommand);
    MethodInfo withPlay = commandType.GetMethod(nameof(AttackCommand.FromCard), [typeof(CardModel), typeof(CardPlay)]);
    if (withPlay != null) {
      _fromCardWithPlay = withPlay.CreateDelegate<Func<AttackCommand, CardModel, CardPlay, AttackCommand>>();
    } else if (commandType.GetMethod(nameof(AttackCommand.FromCard), [typeof(CardModel)]) is MethodInfo withoutPlay) {
      _fromCardWithoutPlay = withoutPlay.CreateDelegate<Func<AttackCommand, CardModel, AttackCommand>>();
    } else {
      throw new MissingMethodException(commandType.FullName, nameof(AttackCommand.FromCard));
    }

    Type creatureCmdType = typeof(CreatureCmd);
    MethodInfo cardWithPlay = creatureCmdType.GetMethod(nameof(CreatureCmd.Damage),
      [typeof(PlayerChoiceContext), typeof(Creature), typeof(DamageVar), typeof(Creature), typeof(CardModel), typeof(CardPlay)]);
    if (cardWithPlay != null) {
      _damageCardWithPlay = cardWithPlay.CreateDelegate<Func<PlayerChoiceContext, Creature, DamageVar, Creature, CardModel, CardPlay, Task<IEnumerable<DamageResult>>>>();
    } else if (creatureCmdType.GetMethod(nameof(CreatureCmd.Damage),
      [typeof(PlayerChoiceContext), typeof(Creature), typeof(DamageVar), typeof(Creature), typeof(CardModel)]) is MethodInfo cardWithoutPlay) {
      _damageCardWithoutPlay = cardWithoutPlay.CreateDelegate<Func<PlayerChoiceContext, Creature, DamageVar, Creature, CardModel, Task<IEnumerable<DamageResult>>>>();
    } else {
      throw new MissingMethodException(creatureCmdType.FullName, nameof(CreatureCmd.Damage));
    }

    MethodInfo targetsWithPlay = creatureCmdType.GetMethod(nameof(CreatureCmd.Damage),
      [typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay)]);
    if (targetsWithPlay != null) {
      _damageTargetsWithPlay = targetsWithPlay.CreateDelegate<Func<PlayerChoiceContext, IEnumerable<Creature>, decimal, ValueProp, Creature, CardModel, CardPlay, Task<IEnumerable<DamageResult>>>>();
    } else if (creatureCmdType.GetMethod(nameof(CreatureCmd.Damage),
      [typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel)]) is MethodInfo targetsWithoutPlay) {
      _damageTargetsWithoutPlay = targetsWithoutPlay.CreateDelegate<Func<PlayerChoiceContext, IEnumerable<Creature>, decimal, ValueProp, Creature, CardModel, Task<IEnumerable<DamageResult>>>>();
    } else {
      throw new MissingMethodException(creatureCmdType.FullName, nameof(CreatureCmd.Damage));
    }
  }

  public static AttackCommand FromCardCompat(this AttackCommand command, CardModel card, CardPlay play) {
    if (_fromCardWithPlay != null) return _fromCardWithPlay(command, card, play);
    return _fromCardWithoutPlay(command, card);
  }

  public static Task<IEnumerable<DamageResult>> DamageCard(PlayerChoiceContext context, Creature target,
      DamageVar damage, Creature dealer, CardModel card, CardPlay play) {
    if (_damageCardWithPlay != null) return _damageCardWithPlay(context, target, damage, dealer, card, play);
    return _damageCardWithoutPlay(context, target, damage, dealer, card);
  }

  public static Task<IEnumerable<DamageResult>> DamageTargets(PlayerChoiceContext context, IEnumerable<Creature> targets,
      decimal amount, ValueProp props, Creature dealer, CardModel card, CardPlay play = null) {
    if (_damageTargetsWithPlay != null) return _damageTargetsWithPlay(context, targets, amount, props, dealer, card, play);
    return _damageTargetsWithoutPlay(context, targets, amount, props, dealer, card);
  }

  public static CardPlay CreateAutoPlay(CardModel card, PileType resultPile, Player player) {
    CardPlay play = Activator.CreateInstance<CardPlay>();
    Set(nameof(CardPlay.Card), card);
    Set(nameof(CardPlay.Target), null);
    Set(nameof(CardPlay.ResultPile), resultPile);
    Set(nameof(CardPlay.Resources), new ResourceInfo { EnergySpent = 0, EnergyValue = 0, StarsSpent = 0, StarValue = 0 });
    Set(nameof(CardPlay.IsAutoPlay), true);
    Set(nameof(CardPlay.PlayIndex), 0);
    Set(nameof(CardPlay.PlayCount), 1);
    if (typeof(CardPlay).GetProperty("Player") is PropertyInfo playerProperty)
      playerProperty.SetValue(play, player);
    return play;

    void Set(string name, object value) {
      PropertyInfo property = typeof(CardPlay).GetProperty(name)
        ?? throw new MissingMemberException(typeof(CardPlay).FullName, name);
      property.SetValue(play, value);
    }
  }

  public static void PlaySpineAnimation(SpineAnimationAccess spine, string animation, string idleAnimation) {
    MegaAnimationState state = spine.GetAnimationState();
    if (state == null) return;

    _setSpineAnimation.Invoke(state, [animation, false, 0]);
    _addSpineAnimation.Invoke(state, [idleAnimation, 0f, true, 0]);
  }
}
