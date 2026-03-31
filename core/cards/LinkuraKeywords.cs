using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace RuriMegu.Core.Cards;
#pragma warning disable CA2211

public static class LinkuraKeywords {

  [CustomEnum("Collect"), KeywordProperties(AutoKeywordPosition.None)]
  public static CardKeyword Collect;

  [CustomEnum("Backstage"), KeywordProperties(AutoKeywordPosition.None)]
  public static CardKeyword Backstage;
}
