using BaseLib.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards;

public class AutoBurstVar : DynamicVar {
  public const string Key = "RURIMEGU-AUTO_BURST";

  public static readonly string LocKey = Key.ToUpperInvariant();
  public static IHoverTip HoverTip() => LocKey.HoverTip(new AutoBurstVar(1));

  public AutoBurstVar(decimal baseValue) : base(Key, baseValue) {
    this.WithTooltip(LocKey);
  }
}
