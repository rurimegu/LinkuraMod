using BaseLib.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards;

public class BurstHeartsVar : DynamicVar {
  public const string Key = "RURIMEGU-BURST";

  public static readonly string LocKey = Key.ToUpperInvariant();
  public static IHoverTip HoverTip(int burstAmount = 1) => LocKey.HoverTip(new BurstHeartsVar(burstAmount));

  public BurstHeartsVar(decimal baseValue) : base(Key, baseValue) {
    this.WithTooltip(LocKey);
  }
}
