using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RuriMegu.Core.Cards;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers;

public class AutoBurstPower : LinkuraPower {
  public const string LocKey = "RURIMEGU-AUTO_BURST_POWER";
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  public override string CustomPackedIconPath => "auto_burst.png".PowerImagePath();
  public override string CustomBigIconPath => "auto_burst.png".PowerImagePath();
  protected override IEnumerable<IHoverTip> ExtraHoverTips => base.ExtraHoverTips.Append(
    BurstHeartsVar.HoverTip());

  public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay) {
    if (Amount > 0 && cardPlay.Card.Owner?.Creature == Owner) {
      await LinkuraCmd.TriggerAutoBurst(cardPlay.Card.Owner, context, null);
    }
  }
}
