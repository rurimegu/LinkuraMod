

using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using RuriMegu.Core.Cards;
using RuriMegu.Core.Characters.Kaho;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Relics.Kaho;

/// <summary>
/// Linkura System - Starter relic for Hinoshita Kaho.
/// Manages the full subscription lifecycle for all LinkuraCards:
/// init at combat start, re-init for mid-combat additions, cleanup on removal or end.
/// </summary>
[Pool(typeof(KahoRelicPool))]
public class LinkuraSystem : LinkuraStarterRelic {
  public override string CharacterId => HinoshitaKaho.CHARACTER_ID;

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromPower<AutoBurstPower>(),
    BurstHeartsVar.HoverTip(),
  ];

  public override async Task BeforeCombatStartLate() {
    await HeartsState.Reset(Owner, new BlockingPlayerChoiceContext());
    await LinkuraCmd.GainAutoBurst(Owner.Creature, 1, Owner.Creature, null);
    Flash();
    await base.BeforeCombatStartLate();
  }
}
