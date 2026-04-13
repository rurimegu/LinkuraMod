using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using RuriMegu.Core.Cards;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Relics.Kaho.Common;

/// <summary>
/// Entrance Exam — Common relic for Hinoshita Kaho.
/// At the start of each combat, Burst 9.
/// </summary>
public class EntranceExam : KahoRelic {
  public override RelicRarity Rarity => RelicRarity.Common;

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    BurstHeartsVar.HoverTip(9),
  ];

  public override async Task BeforeCombatStartLate() {
    Flash();
    await LinkuraCmd.BurstHearts(Owner, Events.BLOCKING_CONTEXT, 9);
    await base.BeforeCombatStartLate();
  }
}
