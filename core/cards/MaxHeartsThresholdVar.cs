using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace RuriMegu.Core.Cards;

public class MaxHeartsThresholdVar(decimal baseValue) : DynamicVar(Key, baseValue) {
  public const string Key = "RURIMEGU-MAX_HEARTS_THRESHOLD";
}
