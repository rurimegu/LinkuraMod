using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace RuriMegu.Core.Relics.Kaho.Shop;

/// <summary>
/// Cap Liberator — Shop relic for Hinoshita Kaho.
/// On pickup: for every pair of identical unupgraded cards in your deck,
/// remove both and add one upgraded copy.
/// </summary>
public class CapLiberator : KahoRelic {
  public override RelicRarity Rarity => RelicRarity.Shop;

  public override bool HasUponPickupEffect => true;

  public override async Task AfterObtained() {
    var toRemove = PileType.Deck.GetPile(Owner).Cards
      .Where(c => !c.IsUpgraded && c.IsUpgradable)
      .GroupBy(c => c.Id)
      .SelectMany(g => g.Take(g.Count() / 2 * 2)) // Take pairs
      .ToList();
    await CardPileCmd.RemoveFromDeck(toRemove);
    var toAdd = toRemove
      .GroupBy(c => c.Id)
      .SelectMany(g => Enumerable.Repeat(g.Key, g.Count() / 2)) // Add one upgraded copy for each pair
      .Select(id => Owner.RunState.CreateCard(ModelDb.GetById<CardModel>(id), Owner))
      .ToList();
    toAdd.ForEach(c => CardCmd.Upgrade(c, CardPreviewStyle.None));
    var addResults = await CardPileCmd.Add(toAdd, PileType.Deck, CardPilePosition.Bottom, this);
    CardCmd.PreviewCardPileAdd(addResults);
  }
}
