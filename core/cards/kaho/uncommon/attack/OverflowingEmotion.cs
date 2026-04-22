using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Attack;

/// Overflowing Emotion: Cost 2, Attack, Uncommon, Ethereal.
/// Deal 9 (12) damage.
/// Backstage: whenever you Collect, permanently gain +2 (+3) damage.
/// </summary>
public class OverflowingEmotion() : KahoInHandTriggerCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {
  private const string GROWTH_VAR = "OVERFLOWING_EMOTION_GROWTH";

  private int _increasedDamage;
  [SavedProperty]
  public int RuriMeguIncreasedDamage {
    get => _increasedDamage;
    set {
      AssertMutable();
      _increasedDamage = value;
      DynamicVars.CalculationBase.BaseValue = (IsUpgraded ? 12 : 9) + _increasedDamage;
    }
  }

  public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Append(CardKeyword.Ethereal);

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CalculationBaseVar((IsUpgraded ? 12 : 9) + _increasedDamage),
    new ExtraDamageVar(1),
    new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => 0m),
    new DynamicVar(GROWTH_VAR, 2),
  ];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromKeyword(LinkuraKeywords.Collect)
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play.Target, DynamicVars.CalculatedDamage.Calculate(play.Target)).Execute(ctx);
  }

  protected override Task InitializeSubscriptions() {
    TrackSubscription(Events.Collect.SubscribeLate(OnCollectHearts));
    return Task.CompletedTask;
  }

  private async Task OnCollectHearts(Events.CollectEvent ev) {
    if (ev.Player != Owner || ev.Amount <= 0) return;

    await TriggerWithAction(ev.Context, () => {
      int gain = DynamicVars[GROWTH_VAR].IntValue;
      BuffFromCollect(gain);
      (DeckVersion as OverflowingEmotion)?.BuffFromCollect(gain);
      return Task.CompletedTask;
    });
  }

  private void BuffFromCollect(int extraDamage) {
    RuriMeguIncreasedDamage += extraDamage;
  }

  protected override void OnUpgrade() {
    DynamicVars.CalculationBase.UpgradeValueBy(3m);
    DynamicVars[GROWTH_VAR].UpgradeValueBy(1m);
  }
}
