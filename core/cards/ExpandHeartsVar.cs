using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace RuriMegu.Core.Cards;

public class ExpandHeartsVar(decimal baseValue) : DynamicVar(Key, baseValue) {
  public const string Key = "LINKURA_MOD_EXPAND_HEARTS";
}
