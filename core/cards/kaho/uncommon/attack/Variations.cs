using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Attack;

/// <summary>
/// Variations — Cost 1, Attack, Uncommon.
/// Deal 6 (9) damage. Draw 1 card.
/// Backstage: whenever max ❤️ changes, this card costs 1 less next time it is played.
/// </summary>
public class Variations() : KahoInHandTriggerCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(6, ValueProp.Move),
    new CardsVar(1),
    new EnergyVar(1),
  ];

  private bool _costReduced = false;
  protected override bool ShouldGlowGoldInternal => _costReduced;

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    _costReduced = false;
    await CommonActions.CardAttack(this, play.Target).Execute(ctx);
    await CommonActions.Draw(this, ctx);
  }

  protected override Task InitializeSubscriptions() {
    TrackSubscription(Events.MaxHeartsChanged.SubscribeLate(OnMaxHeartsChanged));
    return Task.CompletedTask;
  }

  private async Task OnMaxHeartsChanged(Events.MaxHeartsChangedEvent ev) {
    if (ev.Player != Owner || ev.NewMaxHearts == ev.OldMaxHearts) return;
    await TriggerWithAction(ev.Context, () => {
      EnergyCost.AddUntilPlayed(-1, true);
      _costReduced = true;
      InvokeEnergyCostChanged();
      return Task.CompletedTask;
    });
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(3m);
  }
}
