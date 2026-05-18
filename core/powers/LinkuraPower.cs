using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;
using RuriMegu.Core.Utils;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RuriMegu.Core.Powers;

[RegisterPower(Inherit = true)]
public abstract class LinkuraPower : ModPowerTemplate {
  public override PowerType Type => PowerType.None;
  public override PowerStackType StackType => PowerStackType.None;

  public virtual string CharacterId => "";
  public override string CustomIconPath => $"{GetType().Name.PascalToSnakeCase().RemoveSuffix("_power")}.png".PowerImagePath(CharacterId);
  public override string CustomBigIconPath => $"{GetType().Name.PascalToSnakeCase().RemoveSuffix("_power")}.png".PowerImagePath(CharacterId);

  private List<Subscription> _subs = [];

  protected override void DeepCloneFields() {
    base.DeepCloneFields();
    _subs = [];
  }

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
