using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Attack;

/// Overflowing Emotion — Cost 2, Attack, Uncommon, Ethereal, Exhaust.
/// Deal 9 (12) damage.
/// Backstage: whenever you Collect, permanently gain +2 (+3) damage.
/// </summary>
public class OverflowingEmotion() : InHandTriggerCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {
  private const int BASE_DAMAGE = 9;
  private const int UPGRADED_BASE_DAMAGE = 12;
  private const string GROWTH_VAR = "OVERFLOWING_EMOTION_GROWTH";

  private int _currentDamage = BASE_DAMAGE;
  private int _increasedDamage;

  private Subscription _collectSubscription;

  [SavedProperty]
  public int CurrentDamage {
    get {
      return _currentDamage;
    }
    set {
      AssertMutable();
      _currentDamage = value;
      DynamicVars.Damage.BaseValue = _currentDamage;
    }
  }

  [SavedProperty]
  public int IncreasedDamage {
    get {
      return _increasedDamage;
    }
    set {
      AssertMutable();
      _increasedDamage = value;
    }
  }

  public override IEnumerable<CardKeyword> CanonicalKeywords => [
    LinkuraKeywords.Backstage,
    CardKeyword.Exhaust,
    CardKeyword.Ethereal,
  ];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(CurrentDamage, ValueProp.Move),
    new DynamicVar(GROWTH_VAR, 2),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await DamageCmd.Attack(DynamicVars.Damage.IntValue)
      .FromCard(this)
      .Targeting(play.Target)
      .Execute(ctx);
  }

  public override Task BeforeCombatStartLate() {
    _collectSubscription = Events.Collect.SubscribeLate(OnCollectHearts);
    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(MegaCrit.Sts2.Core.Rooms.CombatRoom room) {
    _collectSubscription?.Dispose();
    _collectSubscription = null;
    return Task.CompletedTask;
  }

  private async Task OnCollectHearts(Events.CollectEvent ev) {
    if (ev.Player != Owner) return;

    var triggerEv = await TryTrigger(ev.Context);
    if (triggerEv.IsNullOrCancelled()) return;

    int gain = DynamicVars[GROWTH_VAR].IntValue;
    BuffFromCollect(gain);
    (DeckVersion as OverflowingEmotion)?.BuffFromCollect(gain);
    await AfterTrigger(triggerEv);
  }

  private void BuffFromCollect(int extraDamage) {
    IncreasedDamage += extraDamage;
    UpdateDamage();
  }

  private void UpdateDamage() {
    CurrentDamage = (IsUpgraded ? UPGRADED_BASE_DAMAGE : BASE_DAMAGE) + IncreasedDamage;
  }

  protected override void OnUpgrade() {
    DynamicVars[GROWTH_VAR].UpgradeValueBy(1m);
    UpdateDamage();
  }

  protected override void AfterDowngraded() {
    UpdateDamage();
  }
}
