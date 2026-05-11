

using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using RuriMegu.Core.Cards;
using RuriMegu.Core.Characters.Kaho;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Utils;
using STS2RitsuLib.Interop.AutoRegistration;

namespace RuriMegu.Core.Relics.Kaho.Starter;

public abstract class LinkuraSystemBase : LinkuraStarterRelic {
  public override string CharacterId => HinoshitaKaho.CHARACTER_ID;
  protected abstract int AutoBurstAmount { get; }

  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
    HoverTipFactory.FromPower<AutoBurstPower>(),
    BurstHeartsVar.HoverTip(),
  ];

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new AutoBurstVar(AutoBurstAmount),
  ];

  public override async Task BeforeCombatStartLate() {
    var ctx = new BlockingPlayerChoiceContext();
    await HeartsState.Reset(Owner, ctx);
    await LinkuraCmd.GainAutoBurst(Owner.Creature, ctx, DynamicVars.AutoBurst().IntValue, Owner.Creature, null);
    Flash();
    await base.BeforeCombatStartLate();
  }
}

/// <summary>
/// Linkura System - Starter relic for Hinoshita Kaho.
/// </summary>
[RegisterRelic(typeof(KahoRelicPool))]
[RegisterCharacterStarterRelic(typeof(HinoshitaKaho), 0)]
[RegisterTouchOfOrobasRefinement(typeof(DreamDefine))]
public class LinkuraSystem : LinkuraSystemBase {
  protected override int AutoBurstAmount => 1;
}

[RegisterRelic(typeof(KahoRelicPool))]
[RegisterCharacterStarterRelic(typeof(HinoshitaKaho), 0)]
public class DreamDefine : LinkuraSystemBase {
  protected override int AutoBurstAmount => 3;
}
