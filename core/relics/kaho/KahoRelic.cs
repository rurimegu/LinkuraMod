using BaseLib.Utils;
using RuriMegu.Core.Characters.Kaho;

namespace RuriMegu.Core.Relics.Kaho;

/// <summary>
/// Base class for all Kaho relics.
/// Inheriting from this automatically places relics in the Kaho relic pool.
/// </summary>
[Pool(typeof(KahoRelicPool))]
public abstract class KahoRelic : LinkuraRelic {
  public override string CharacterId => "kaho";
}
