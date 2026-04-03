using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers;

public class AutoBurstPower : LinkuraPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  public override string CustomPackedIconPath => "auto_burst.png".PowerImagePath();
  public override string CustomBigIconPath => "auto_burst.png".PowerImagePath();

  public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay) {
    if (Amount > 0 && cardPlay.Card.Owner?.Creature == Owner) {
      await LinkuraCmd.TriggerAutoBurst(cardPlay.Card.Owner, context, null);
    }
  }
}
