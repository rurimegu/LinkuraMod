using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace RuriMegu.Core.Cards;

public class AutoBurstVar : DynamicVar {
  public const string Key = "RURIMEGU-AUTO_BURST";

  public static readonly string LocKey = Key.ToUpperInvariant();

  public AutoBurstVar(decimal baseValue) : base(Key, baseValue) {
    this.WithTooltip(LocKey);
  }
}
