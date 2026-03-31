using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace RuriMegu.Core.Cards;

public class BurstHeartsVar : DynamicVar {
  public const string Key = "RURIMEGU-BURST";

  public static readonly string LocKey = Key.ToUpperInvariant();

  public BurstHeartsVar(decimal baseValue) : base(Key, baseValue) {
    this.WithTooltip(LocKey);
  }
}
