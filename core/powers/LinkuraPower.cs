using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Powers;

public abstract class LinkuraPower : CustomPowerModel {
  public override PowerType Type => PowerType.None;
  public override PowerStackType StackType => PowerStackType.None;

  public virtual string CharacterId => "";
  public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant().RemoveSuffix("_power")}.png".PowerImagePath(CharacterId);
  public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant().RemoveSuffix("_power")}.png".PowerImagePath(CharacterId);

  private readonly List<Subscription> _subs = [];

  protected void TrackSubscription(Subscription sub) => _subs.Add(sub);

  protected void DisposeTrackedSubscriptions() {
    foreach (var sub in _subs) sub.Dispose();
    _subs.Clear();
  }

  public override Task AfterRemoved(Creature oldOwner) {
    DisposeTrackedSubscriptions();
    return base.AfterRemoved(oldOwner);
  }

  public override Task AfterCombatEnd(CombatRoom room) {
    DisposeTrackedSubscriptions();
    return base.AfterCombatEnd(room);
  }
}
