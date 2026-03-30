using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace RuriMegu.Core.Powers;

public abstract class LinkuraPower : CustomPowerModel {
  public override PowerType Type => PowerType.None;
  public override PowerStackType StackType => PowerStackType.None;
}
