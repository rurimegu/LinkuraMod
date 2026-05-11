using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;
using STS2RitsuLib.Keywords;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Love it! — Cost 1, Skill, Common.
/// Burst 6 (9). Draw 1 (2) cards.
/// Backstage: whenever you Collect, gain 3 (5) block.
/// </summary>
public class LoveIt() : KahoInHandTriggerCard(1, CardType.Skill, CardRarity.Common, TargetType.None) {

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BurstHeartsVar(6),
    new CardsVar(1),
    new BlockVar(3, ValueProp.Move),
  ];

  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
    ModKeywordRegistry.CreateHoverTip(LinkuraKeywords.Collect),
    HoverTipFactory.Static(StaticHoverTip.Block),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.BurstHearts(this, ctx);
    await CommonActions.Draw(this, ctx);
  }

  protected override Task InitializeSubscriptions() {
    TrackSubscription(Events.Collect.SubscribeLate(OnCollectHearts));
    return Task.CompletedTask;
  }

  private async Task OnCollectHearts(Events.CollectEvent ev) {
    if (ev.Player != Owner) return;
    await TriggerWithAction(ev.Context, () =>
      CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null));
  }

  protected override void OnUpgrade() {
    DynamicVars.BurstHearts().UpgradeValueBy(3m);
    DynamicVars.Cards.UpgradeValueBy(1m);
    DynamicVars.Block.UpgradeValueBy(2m);
  }
}
