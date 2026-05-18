using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RuriMegu.Core.Cards;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers;

public class AutoBurstPower : LinkuraPower {
  public const string LocKey = "LINKURA_MOD_AUTO_BURST_POWER";
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
    BurstHeartsVar.HoverTip(),
  ];

  public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay) {
    if (Amount > 0 && cardPlay.Card.Owner?.Creature == Owner) {
      await LinkuraCmd.TriggerAutoBurst(cardPlay.Card.Owner, context, null);
    }
  }
}
