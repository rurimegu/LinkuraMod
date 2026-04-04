using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Attack;

/// <summary>
/// Afterglow — Cost 0, Attack, Uncommon.
/// Deal 4 (6) damage.
/// Whenever you Collect, return this card from discard pile to hand.
/// </summary>
public class Afterglow() : LinkuraCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) {
  private Subscription _collectHeartsSubscription;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(4, ValueProp.Move),
  ];
  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromKeyword(LinkuraKeywords.Collect)
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    await CommonActions.CardAttack(this, play.Target).Execute(ctx);
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
    if (!this.IsInDiscardPile()) return;
    await CardPileCmd.Add(this, PileType.Hand);
  }

  protected override void OnUpgrade() {
    DynamicVars.Damage.UpgradeValueBy(2m);
  }
}
