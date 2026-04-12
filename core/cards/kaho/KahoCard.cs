using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using RuriMegu.Core.Characters.Kaho;

namespace RuriMegu.Core.Cards.Kaho;

[Pool(typeof(Characters.Kaho.HinoshitaKahoCardPool))]
public abstract class KahoCard(int cost, CardType type, CardRarity rarity, TargetType target)
    : LinkuraCard(cost, type, rarity, target) {
  public override string CharacterId => HinoshitaKaho.CHARACTER_ID;
}
