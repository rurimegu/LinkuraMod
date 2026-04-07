using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers;

/// <summary>
/// At the start of your turn, Burst Hearts equal to <c>Amount</c>.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Common.Power.GlowingRoutine"/>.
/// </summary>
public class GlowingRoutinePower : LinkuraPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player) {
    await base.AfterPlayerTurnStart(choiceContext, player);
    if (player != Owner.Player) return;
    Flash();
    await LinkuraCmd.BurstHearts(Owner.Player, choiceContext, Amount);
  }
}
