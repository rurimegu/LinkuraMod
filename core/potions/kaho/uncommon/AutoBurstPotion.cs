using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RuriMegu.Core.Cards;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Potions.Kaho.Uncommon;

/// <summary>
/// Auto Burst Potion — Uncommon potion for Hinoshita Kaho.
/// Gain 2 stacks of Auto Burst.
/// </summary>
public class AutoBurstPotion : KahoPotion {
  private const int AUTO_BURST_AMOUNT = 2;

  public override PotionRarity Rarity => PotionRarity.Uncommon;
  public override PotionUsage Usage => PotionUsage.CombatOnly;
  public override TargetType TargetType => TargetType.None;

  protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.AdditionalHoverTips.Concat([
    HoverTipFactory.FromPower<AutoBurstPower>(),
    BurstHeartsVar.HoverTip()
  ]);

  protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature target) {
    await LinkuraCmd.GainAutoBurst(Owner.Creature, choiceContext, AUTO_BURST_AMOUNT, Owner.Creature, null);
  }
}
