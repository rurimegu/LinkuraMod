using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Cards.Kaho.Uncommon.Skill;

/// <summary>
/// On Your Mark — X Cost, Skill, Uncommon.
/// Increase max ❤️ by 4 (6) X. When drawn, gain Block equal to 2(3)x your {Energy:energyIcons()}.
/// </summary>
public class OnYourMark() : KahoCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.None) {
  protected override bool HasEnergyCostX => true;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new ExpandHeartsVar(4),
    new BlockVar(2, ValueProp.Unpowered),
  ];

  protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) {
    int heartIncrease = DynamicVars.ExpandHearts().IntValue * ResolveEnergyXValue();
    if (heartIncrease > 0) {
      await LinkuraCmd.IncreaseMaxHearts(Owner, ctx, heartIncrease, this);
    }
  }

  public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw) {
    if (card == this) {
      await Cmd.Wait(0.25f);
      int energy = Owner.PlayerCombatState?.Energy ?? 0;
      int block = energy * DynamicVars.Block.IntValue;
      if (block > 0) {
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, null);
      }
    }
  }

  protected override void OnUpgrade() {
    DynamicVars.ExpandHearts().UpgradeValueBy(2m);
    DynamicVars.Block.UpgradeValueBy(1m);
  }
}
