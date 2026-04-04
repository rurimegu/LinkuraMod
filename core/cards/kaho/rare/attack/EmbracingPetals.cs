using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Rare.Attack;

/// <summary>
/// Embracing Petals — Cost 2, Attack, Rare.
/// Deal damage equal to your Auto Burst amount to ALL enemies 6 (9) times. Backstage: whenever you Collect, gain 1 Auto Burst.
/// </summary>
public class EmbracingPetals() : InHandTriggerCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) {
  private Subscription _collectHeartsSubscription;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CalculationBaseVar(0),
    new ExtraDamageVar(1),
    new CalculatedDamageVar(ValueProp.Move).WithMultiplier((_, creature) => creature.GetPowerAmount<AutoBurstPower>()),
    new AutoBurstVar(1),
    new RepeatVar(6),
  ];

  public override Task BeforeCombatStartLate() {
    _collectHeartsSubscription = Events.Collect.SubscribeLate(OnCollectHearts);
    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    _collectHeartsSubscription?.Dispose();
    _collectHeartsSubscription = null;
    return Task.CompletedTask;
  }

  private async Task OnCollectHearts(Events.CollectEvent ev) {
    if (ev.Player != Owner || ev.Amount <= 0) return;

    await TriggerWithAction(ev.Context, () => PowerCmd.Apply<AutoBurstPower>(Owner.Creature, DynamicVars.AutoBurst().IntValue, Owner.Creature, this));
  }

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int hitCount = DynamicVars.Repeat.IntValue;
    await CommonActions.CardAttack(this, play, hitCount: hitCount).Execute(ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.Repeat.UpgradeValueBy(3m);
  }
}
