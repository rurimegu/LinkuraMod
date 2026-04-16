

using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
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

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new AutoBurstVar(AutoBurstAmount),
  ];

  public override async Task BeforeCombatStartLate() {
    await HeartsState.Reset(Owner, Events.BLOCKING_CONTEXT);
    await LinkuraCmd.GainAutoBurst(Owner.Creature, DynamicVars.AutoBurst().IntValue, Owner.Creature, null);
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
