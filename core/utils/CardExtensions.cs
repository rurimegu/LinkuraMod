using System.Linq;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Cards;

namespace RuriMegu.Core.Utils;

public static class CardExtensions {
  public static DynamicVar ExpandHearts(this DynamicVarSet vars) {
    if (!vars.TryGetValue(ExpandHeartsVar.Key, out var value)) {
      LinkuraMod.Logger.Error($"ExpandHeartsVar not found for card!");
      return null;
    }
    return value;
  }

  public static DynamicVar BurstHearts(this DynamicVarSet vars) {
    if (!vars.TryGetValue(BurstHeartsVar.Key, out var value)) {
      LinkuraMod.Logger.Error($"BurstHeartsVar not found for card!");
      return null;
    }
    return value;
  }

  public static DynamicVar AutoBurst(this DynamicVarSet vars) {
    if (!vars.TryGetValue(AutoBurstVar.Key, out var value)) {
      LinkuraMod.Logger.Error($"AutoBurstVar not found for card!");
      return null;
    }
    return value;
  }

  public static DynamicVar TriggerAutoBurst(this DynamicVarSet vars) {
    if (!vars.TryGetValue(TriggerAutoBurstVar.Key, out var value)) {
      LinkuraMod.Logger.Error($"TriggerAutoBurstVar not found for card!");
      return null;
    }
    return value;
  }

  public static DynamicVar MaxHeartThreshold(this DynamicVarSet vars) {
    if (!vars.TryGetValue(MaxHeartsThresholdVar.Key, out var value)) {
      LinkuraMod.Logger.Error($"MaxHeartThresholdVar not found for card!");
      return null;
    }
    return value;
  }


  public static bool IsInHand(this CardModel card) {
    CardPile handPile = PileType.Hand.GetPile(card.Owner);
    return handPile != null && handPile.Cards.Contains(card);
  }

  public static bool IsInDrawPile(this CardModel card) {
    CardPile drawPile = PileType.Draw.GetPile(card.Owner);
    return drawPile != null && drawPile.Cards.Contains(card);
  }

  public static bool IsInDiscardPile(this CardModel card) {
    CardPile discardPile = PileType.Discard.GetPile(card.Owner);
    return discardPile != null && discardPile.Cards.Contains(card);
  }
}
