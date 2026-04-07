using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers;

/// <summary>
/// Section Change — At the end of your turn, Collect.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Common.Power.SectionChange"/>.
/// </summary>
public class SectionChangePower : LinkuraPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;

  public override async Task BeforeTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side) {
    await base.BeforeTurnEndEarly(choiceContext, side);
    if (side != Owner.Side) return;
    Flash();
    await LinkuraCmd.CollectHearts(Owner.Player, choiceContext);
  }
}
