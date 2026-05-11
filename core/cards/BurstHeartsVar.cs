using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Utils;
using STS2RitsuLib.Cards.DynamicVars;

namespace RuriMegu.Core.Cards;

public class BurstHeartsVar : DynamicVar {
  public const string Key = "LINKURA_MOD_BURST";

  public static IHoverTip HoverTip(int burstAmount = 1) => Key.HoverTip(new BurstHeartsVar(burstAmount));

  public BurstHeartsVar(decimal baseValue) : base(Key, baseValue) {
    this.WithSharedTooltip(Key);
  }
}
