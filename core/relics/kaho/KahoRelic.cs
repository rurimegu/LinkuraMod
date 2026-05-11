using RuriMegu.Core.Characters.Kaho;
using STS2RitsuLib.Interop.AutoRegistration;

namespace RuriMegu.Core.Relics.Kaho;

/// <summary>
/// Base class for all Kaho relics.
/// Inheriting from this automatically places relics in the Kaho relic pool.
/// </summary>
[RegisterRelic(typeof(KahoRelicPool), Inherit = true)]
public abstract class KahoRelic : LinkuraRelic {
  public override string CharacterId => HinoshitaKaho.CHARACTER_ID;
}
