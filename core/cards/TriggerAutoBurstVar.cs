using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace RuriMegu.Core.Cards;

public class TriggerAutoBurstVar : DynamicVar {
  public const string Key = "RURIMEGU-AUTO_BURST_TRIGGERS";

  public static readonly string LocKey = Key.ToUpperInvariant();

  public TriggerAutoBurstVar(decimal baseValue) : base(Key, baseValue) {
    this.WithTooltip(LocKey);
  }
}
