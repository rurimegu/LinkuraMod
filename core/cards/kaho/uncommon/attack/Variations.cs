using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Attack;

/// <summary>
/// Variations — Cost 1, Attack, Uncommon.
/// Deal 4 (6) damage. Draw 1 card.
/// Backstage: whenever max ❤️ changes, this card costs 1 less next time it is played.
/// </summary>
public class Variations() : InHandTriggerCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {
  private Subscription _maxHeartsSubscription;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(4, ValueProp.Move),
    new CardsVar(1),
    new EnergyVar(1),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play.Target).Execute(ctx);
    await CommonActions.Draw(this, ctx);
  }

  public override Task BeforeCombatStartLate() {
    _maxHeartsSubscription = Events.MaxHeartsChanged.SubscribeLate(OnMaxHeartsChanged);
    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    _maxHeartsSubscription?.Dispose();
    _maxHeartsSubscription = null;
    return Task.CompletedTask;
  }

  private async Task OnMaxHeartsChanged(Events.MaxHeartsChangedEvent ev) {
    if (ev.Player != Owner) return;
    var triggerEv = await TryTrigger(ev.Context);
    if (triggerEv.IsNullOrCancelled()) return;

    EnergyCost.AddUntilPlayed(-1, true);
    InvokeEnergyCostChanged();
    await AfterTrigger(triggerEv);
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(2m);
  }
}
