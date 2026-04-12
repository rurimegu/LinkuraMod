using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using RuriMegu.Core.Cards;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Relics;

public abstract class LinkuraStarterRelic : LinkuraRelic {
  public override RelicRarity Rarity => RelicRarity.Starter;

  public override async Task BeforeCombatStartLate() {
    foreach (var pile in Owner.PlayerCombatState.AllPiles)
      foreach (var card in pile.Cards)
        if (card is LinkuraCard lc) await lc.RunInitializeSubscriptions();
    await base.BeforeCombatStartLate();
  }

  public override Task AfterCardEnteredCombat(CardModel card) {
    if (card is LinkuraCard lc && card.Owner == Owner)
      return lc.RunInitializeSubscriptions();
    return Task.CompletedTask;
  }

  public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel source) {
    if (card is LinkuraCard lc && lc.Pile == null)
      lc.DisposeTrackedSubscriptions();
    return base.AfterCardChangedPiles(card, oldPileType, source);
  }

  public override Task AfterCombatEnd(CombatRoom room) {
    foreach (var pile in Owner.PlayerCombatState?.AllPiles ?? [])
      foreach (var card in pile.Cards)
        if (card is LinkuraCard lc) lc.DisposeTrackedSubscriptions();
    return base.AfterCombatEnd(room);
  }
}
