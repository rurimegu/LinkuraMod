using MegaCrit.Sts2.Core.Entities.Powers;

namespace RuriMegu.Core.Powers.Kaho;

/// <summary>
/// Allows Backstage cards in the discard pile to trigger their in-hand effects.
/// Applied by <see cref="RuriMegu.Core.Cards.Kaho.Rare.Power.BloomGardenParty"/>.
/// </summary>
public class BloomGardenPartyPower : KahoPower {
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Single;
}
