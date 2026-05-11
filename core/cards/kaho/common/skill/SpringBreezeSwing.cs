using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Spring Breeze Swing (春风秋千) — Cost 1, Skill, Common.
/// Draw 1 (2) card.
/// Backstage: whenever Burst Hearts reaches the ♥ limit, gain 1 energy.
/// </summary>
public class SpringBreezeSwing() : KahoInHandTriggerCard(1, CardType.Skill, CardRarity.Common, TargetType.None) {


  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new CardsVar(1),
    new EnergyVar(1),
  ];

  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [BurstHeartsVar.HoverTip()];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.Draw(this, ctx);
  }

  protected override Task InitializeSubscriptions() {
    TrackSubscription(Events.Burst.SubscribeLate(OnBurstHearts));
    return Task.CompletedTask;
  }

  private async Task OnBurstHearts(Events.BurstEvent ev) {
    if (ev.Player != Owner || ev.ActualAmount <= 0) return;
    var childEv = ev.HeartsChangedEvent;
    if (childEv == null || childEv.NewHearts < childEv.MaxHearts)
      return;
    await TriggerWithAction(ev.Context, () => PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner));
  }

  protected override void OnUpgrade() {
    DynamicVars.Cards.UpgradeValueBy(1m);
    DynamicVars.Energy.UpgradeValueBy(1m);
  }
}
