using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Relics.Kaho.Uncommon;

/// <summary>
/// The Tale of Peter Rabbit — Uncommon relic for Hinoshita Kaho.
/// If your ❤️ have not reached half the cap at the end of your turn, gain 6 block.
/// </summary>
public class TaleOfPeterRabbit : KahoRelic {
  public override RelicRarity Rarity => RelicRarity.Uncommon;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BlockVar(6m, ValueProp.Unpowered),
  ];

  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
    HoverTipFactory.Static(StaticHoverTip.Block),
  ];

  public override async Task BeforeTurnEnd(PlayerChoiceContext ctx, CombatSide side) {
    if (side != CombatSide.Player) return;
    if (HeartsState.ReachedHalfHearts(Owner)) return;
    Flash();
    await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
  }
}
