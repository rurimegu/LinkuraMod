using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace RuriMegu.Core.Cards;

public class TriggerAutoBurstVar(decimal baseValue) : DynamicVar(Key, baseValue) {
  public const string Key = "RURIMEGU-AUTO_BURST_TRIGGERS";

  public static readonly string LocKey = Key.ToUpperInvariant();
}
