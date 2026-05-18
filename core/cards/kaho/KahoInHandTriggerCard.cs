
using MegaCrit.Sts2.Core.Entities.Cards;
using RuriMegu.Core.Characters.Kaho;
using STS2RitsuLib.Interop.AutoRegistration;

namespace RuriMegu.Core.Cards.Kaho;

[RegisterCard(typeof(HinoshitaKahoCardPool), Inherit = true)]
public abstract class KahoInHandTriggerCard(int cost, CardType type, CardRarity rarity, TargetType target)
    : InHandTriggerCard(cost, type, rarity, target) {
  public override string CharacterId => HinoshitaKaho.CHARACTER_ID;
}
