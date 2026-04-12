using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers.Kaho;

/// <summary>
/// Whenever you Collect Hearts, deal damage to all enemies.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Uncommon.Power.SpecialAppeal"/>.
/// </summary>
public class SpecialAppealPower : KahoPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;
}
