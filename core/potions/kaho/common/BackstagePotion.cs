using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RuriMegu.Core.Cards;
using STS2RitsuLib.Keywords;

namespace RuriMegu.Core.Potions.Kaho.Common;

/// <summary>
/// Backstage Potion — Common potion for Hinoshita Kaho.
/// Choose 1 from 3 random Backstage cards from the card pool and add it to your hand. It gains Retain.
/// </summary>
public class BackstagePotion : KahoPotion {
  private const int PICK_COUNT = 3;

  public override PotionRarity Rarity => PotionRarity.Common;
  public override PotionUsage Usage => PotionUsage.CombatOnly;
  public override TargetType TargetType => TargetType.None;
  protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat([
    ModKeywordRegistry.CreateHoverTip(LinkuraKeywords.Backstage),
    HoverTipFactory.FromKeyword(CardKeyword.Retain)
  ]);

  protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature target) {
    var poolCards = Owner.Character.CardPool
      .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
      .Where(c => c.HasModKeyword(LinkuraKeywords.Backstage));

    var cards = CardFactory.GetDistinctForCombat(Owner, poolCards, PICK_COUNT, Owner.RunState.Rng.CombatCardGeneration).ToList();
    if (cards.Count == 0) return;

    var card = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, Owner, canSkip: true);
    if (card == null) return;

    if (!card.Keywords.Contains(CardKeyword.Retain))
      card.AddKeyword(CardKeyword.Retain);
    await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);
  }
}
