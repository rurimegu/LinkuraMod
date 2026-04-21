using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RuriMegu.Core.Cards;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Relics.Kaho.Common;

/// <summary>
/// Entrance Exam — Common relic for Hinoshita Kaho.
/// At the start of each combat, Burst 9.
/// </summary>
public class EntranceExam : KahoRelic {
  public override RelicRarity Rarity => RelicRarity.Common;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new BurstHeartsVar(9m),
  ];

  protected override IEnumerable<IHoverTip> ExtraHoverTips => base.ExtraHoverTips.Concat([
    BurstHeartsVar.HoverTip()
  ]);

  public override async Task BeforeCombatStartLate() {
    Flash();
    await LinkuraCmd.BurstHearts(Owner, new BlockingPlayerChoiceContext(), DynamicVars[BurstHeartsVar.Key].IntValue);
    await base.BeforeCombatStartLate();
  }
}
