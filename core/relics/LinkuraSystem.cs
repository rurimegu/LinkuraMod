

using System.Threading.Tasks;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Relics;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Relics;

/// <summary>
/// Linkura Charm - Starter relic for Hinoshita Kaho.
/// </summary>
public class LinkuraSystem : LinkuraRelic {
  public override RelicRarity Rarity => RelicRarity.Starter;
  public override bool ShouldReceiveCombatHooks => true;

  public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
  protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
  protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();

  public override Task BeforeCombatStart() {
    return HeartsState.Reset(Owner);
  }
}
