using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers;

/// <summary>
/// At the start of your turn, convert ❤️ to equal Block.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Uncommon.Power.CareerSurvey"/>.
/// </summary>
public class CareerSurveyPower : LinkuraPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;

  public override async Task BeforeSideTurnStart(PlayerChoiceContext ctx, CombatSide side, CombatState combatState) {
    await base.BeforeSideTurnStart(ctx, side, combatState);
    if (side != Owner.Side) return;
    int hearts = HeartsState.GetHearts(Owner.Player);
    if (hearts <= 0) return;
    await HeartsState.SetHearts(Owner.Player, ctx, 0);
    await CreatureCmd.GainBlock(Owner, hearts, ValueProp.Unpowered, null);
  }
}
