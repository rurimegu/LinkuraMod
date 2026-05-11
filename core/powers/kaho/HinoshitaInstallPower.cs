using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RuriMegu.Core.Cards;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers.Kaho;

/// <summary>
/// Whenever another player plays a card, trigger the owner's Auto Burst.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Rare.Power.HinoshitaInstall"/>.
/// </summary>
public class HinoshitaInstallPower : KahoPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;

  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
    HoverTipFactory.FromPower<AutoBurstPower>(),
    BurstHeartsVar.HoverTip(),
  ];

  public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay) {
    if (cardPlay.Card.Owner == null || cardPlay.Card.Owner == Owner.Player) return;
    if (cardPlay.Card.Owner.Creature.Side != CombatSide.Player) return;
    Flash();
    await LinkuraCmd.TriggerAutoBurst(Owner.Player, context, cardPlay.Card);
  }
}
