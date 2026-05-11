using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Utils;
using STS2RitsuLib.Keywords;

namespace RuriMegu.Core.Cards.Kaho.Rare.Attack;

/// <summary>
/// Embracing Petals — Cost 2, Attack, Rare.
/// Deal damage equal to your Auto Burst amount to ALL enemies 6 (9) times. Backstage: whenever you Collect, gain 1 Auto Burst.
/// </summary>
public class EmbracingPetals() : KahoInHandTriggerCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) {


  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CalculationBaseVar(0),
    new ExtraDamageVar(1),
    new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => card.Owner.Creature.GetPowerAmount<AutoBurstPower>()),
    new AutoBurstVar(1),
    new RepeatVar(6),
  ];

  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
    ModKeywordRegistry.CreateHoverTip(LinkuraKeywords.Collect),
    HoverTipFactory.FromPower<AutoBurstPower>(),
    BurstHeartsVar.HoverTip(),
  ];

  protected override Task InitializeSubscriptions() {
    TrackSubscription(Events.Collect.SubscribeLate(OnCollectHearts));
    return Task.CompletedTask;
  }

  private async Task OnCollectHearts(Events.CollectEvent ev) {
    if (ev.Player != Owner || ev.Amount <= 0) return;

    var triggered = await TriggerWithAction(ev.Context, () => LinkuraCmd.GainAutoBurst(Owner.Creature, ev.Context, DynamicVars.AutoBurst().IntValue, Owner.Creature, this));
    if (!triggered.IsNullOrCancelled() && this.IsInHand()) {
      await CardCmd.Discard(ev.Context, this);
    }
  }

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int hitCount = DynamicVars.Repeat.IntValue;
    await CommonActions.CardAttack(this, null, DynamicVars.CalculatedDamage.Calculate(null), hitCount: hitCount).Execute(ctx);
  }

  protected override void OnUpgrade() {
    DynamicVars.Repeat.UpgradeValueBy(3m);
  }
}
