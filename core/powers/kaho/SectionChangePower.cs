using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers.Kaho;

/// <summary>
/// Section Change — At the end of your turn, Collect.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Common.Power.SectionChange"/>.
/// </summary>
public class SectionChangePower : KahoPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;

  public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants) {
    await base.BeforeSideTurnEndEarly(choiceContext, side, participants);
    if (side != Owner.Side) return;
    Flash();
    await LinkuraCmd.CollectHearts(Owner.Player, choiceContext);
  }
}
