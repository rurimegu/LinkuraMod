using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers.Kaho;

/// <summary>
/// When you increase Max ❤️, cancel it and track the highest request. Award Max HP at end of combat.
/// Amount represents the multiplier.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Ancient.TragicNightFireworks"/>.
/// </summary>
public class TragicNightFireworksPower : KahoPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  private const string MAX_INCREASE_VAR = "TRAGIC_NIGHT_FIREWORKS_MAX_INCREASE";

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DynamicVar(MAX_INCREASE_VAR, 0),
  ];

  public override Task AfterApplied(Creature applier, CardModel cardSource) {
    DisposeTrackedSubscriptions();
    TrackSubscription(Events.IncreaseMaxHearts.SubscribeEarly(OnIncreaseMaxHeartsEarly));
    return base.AfterApplied(applier, cardSource);
  }

  private Task OnIncreaseMaxHeartsEarly(Events.IncreaseMaxHeartsEvent ev) {
    if (ev.Player != Owner.Player || ev.RequestedAmount <= 0) return Task.CompletedTask;

    ev.Cancel();
    Flash();

    if (ev.RequestedAmount > DynamicVars[MAX_INCREASE_VAR].BaseValue) {
      DynamicVars[MAX_INCREASE_VAR].BaseValue = ev.RequestedAmount;
    }
    return Task.CompletedTask;
  }

  public override async Task AfterCombatEnd(CombatRoom room) {
    await base.AfterCombatEnd(room);

    int maxIncrease = (int)DynamicVars[MAX_INCREASE_VAR].BaseValue;
    int maxHpGain = maxIncrease * Amount;
    if (maxHpGain > 0) {
      Flash();
      await CreatureCmd.GainMaxHp(Owner, maxHpGain);
    }
  }
}
