using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Attack;

/// <summary>
/// Ephemeral Youth — Cost 1, Attack, Uncommon, Ethereal.
/// Deal 14 (20) damage.
/// Backstage: whenever you play a card, this card's damage this turn is reduced by 4.
/// </summary>
public class EphemeralYouth() : InHandTriggerCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {
  private const string TOTAL_DMG_VAR = "EPHEMERAL_YOUTH_TOTAL_DMG";

  private int _reductionCountThisTurn = 0;

  public override IEnumerable<CardKeyword> CanonicalKeywords => [
    LinkuraKeywords.Backstage,
    CardKeyword.Ethereal,
  ];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CalculationBaseVar(14),
    new ExtraDamageVar(-4),
    new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
      (card, _) => (card as EphemeralYouth)?._reductionCountThisTurn ?? 0),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play.Target).Execute(ctx);
  }

  public override Task BeforeCombatStartLate() {
    _reductionCountThisTurn = 0;
    return Task.CompletedTask;
  }

  public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay) {
    await base.AfterCardPlayed(context, cardPlay);
    if (cardPlay.Card.Owner != Owner) return;
    if (cardPlay.Card == this) return;

    var triggerEv = await TryTrigger(context);
    if (triggerEv.IsNullOrCancelled()) return;

    _reductionCountThisTurn++;
    await AfterTrigger(triggerEv);
  }

  public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side) {
    await base.AfterTurnEnd(choiceContext, side);
    if (side != Owner.Creature.Side) return;
    _reductionCountThisTurn = 0;
  }

  protected override void OnUpgrade() {
    DynamicVars.CalculationBase.UpgradeValueBy(6m);
  }
}
