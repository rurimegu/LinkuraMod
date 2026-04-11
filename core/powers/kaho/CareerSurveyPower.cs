using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers.Kaho;

/// <summary>
/// At the start of your turn, convert 鉂わ笍 to equal Block.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Uncommon.Power.CareerSurvey"/>.
/// </summary>
public class CareerSurveyPower : KahoPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;

  public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player) {
    await base.AfterPlayerTurnStart(ctx, player);
    if (player != Owner.Player) return;
    int hearts = HeartsState.GetHearts(Owner.Player);
    if (hearts <= 0) return;
    await HeartsState.SetHearts(Owner.Player, ctx, 0);
    Flash();
    await CreatureCmd.GainBlock(Owner, hearts, ValueProp.Unpowered, null);
  }
}
