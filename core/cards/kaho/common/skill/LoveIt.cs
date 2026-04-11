using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Common.Skill;

/// <summary>
/// Love it! 鈥?Cost 1, Skill, Common.
/// Burst 6 (9). Draw 1 (2) cards.
/// Backstage: whenever you Collect, gain 4 (6) block.
/// </summary>
public class LoveIt() : KahoInHandTriggerCard(1, CardType.Skill, CardRarity.Common, TargetType.None) {
  private const string BACKSTAGE_BLOCK_VAR = "BACKSTAGE_BLOCK";
  private Subscription _collectHeartsSubscription;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BurstHeartsVar(6),
    new CardsVar(1),
    new DynamicVar(BACKSTAGE_BLOCK_VAR, 4),
  ];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => base.ExtraHoverTips.Concat([
    HoverTipFactory.FromKeyword(LinkuraKeywords.Collect),
    HoverTipFactory.Static(StaticHoverTip.Block),
  ]);

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await LinkuraCardActions.BurstHearts(this, ctx);
    await CommonActions.Draw(this, ctx);
  }

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
    if (ev.Player != Owner) return;
    await TriggerWithAction(ev.Context, () =>
      CreatureCmd.GainBlock(Owner.Creature, DynamicVars[BACKSTAGE_BLOCK_VAR].IntValue, ValueProp.Move, null));
  }

  protected override void OnUpgrade() {
    DynamicVars.BurstHearts().UpgradeValueBy(3m);
    DynamicVars.Cards.UpgradeValueBy(1m);
    DynamicVars[BACKSTAGE_BLOCK_VAR].UpgradeValueBy(2m);
  }
}
