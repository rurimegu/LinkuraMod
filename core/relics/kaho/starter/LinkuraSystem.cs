

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

namespace RuriMegu.Core.Relics.Kaho.Starter;

[Pool(typeof(KahoRelicPool))]
public abstract class LinkuraSystemBase : LinkuraStarterRelic {
  public override string CharacterId => HinoshitaKaho.CHARACTER_ID;
  protected abstract int AutoBurstAmount { get; }

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromPower<AutoBurstPower>(),
    BurstHeartsVar.HoverTip(),
  ];

  public override async Task BeforeCombatStartLate() {
    await HeartsState.Reset(Owner, new BlockingPlayerChoiceContext());
    await LinkuraCmd.GainAutoBurst(Owner.Creature, AutoBurstAmount, Owner.Creature, null);
    Flash();
    await base.BeforeCombatStartLate();
  }
}

/// <summary>
/// Linkura System - Starter relic for Hinoshita Kaho.
/// </summary>
public class LinkuraSystem : LinkuraSystemBase {
  protected override int AutoBurstAmount => 1;
  public override RelicModel GetUpgradeReplacement() => ModelDb.Relic<DreamDefine>();
}

public class DreamDefine : LinkuraSystemBase {
  protected override int AutoBurstAmount => 3;
}
