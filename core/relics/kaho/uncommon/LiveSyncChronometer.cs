using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using RuriMegu.Core.Cards;
using RuriMegu.Core.Powers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Relics.Kaho.Uncommon;

/// <summary>
/// Live-Sync Chronometer — Uncommon relic for Hinoshita Kaho.
/// During your turn, trigger Auto Burst once per real-time minute.
/// Note: only triggered during player's turn to avoid bugs during enemy turn.
/// </summary>
public class LiveSyncChronometer : KahoRelic {
  public override RelicRarity Rarity => RelicRarity.Uncommon;

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromPower<AutoBurstPower>(),
    BurstHeartsVar.HoverTip(),
  ];

  private CancellationTokenSource _cts;
  private int _secondCount = 0;

  [SavedProperty]
  public int RuriMeguSecondCount {
    get => _secondCount;
    set {
      AssertMutable();
      _secondCount = value;
      InvokeDisplayAmountChanged();
    }
  }

  public override bool ShowCounter => true;
  public override int DisplayAmount => _secondCount;

  private void StartLoop() {
    CancelLoop();
    _cts = new CancellationTokenSource();
    _ = RunMinuteLoop(_cts.Token);
  }

  private void CancelLoop() {
    _cts?.Cancel();
    _cts?.Dispose();
    _cts = null;
  }

  public override Task BeforeCombatStart() {
    StartLoop();
    return base.BeforeCombatStart();
  }

  public override Task AfterObtained() {
    if (CombatManager.Instance.IsInProgress) {
      StartLoop();
    }
    return base.AfterObtained();
  }

  public override Task AfterCombatEnd(CombatRoom room) {
    CancelLoop();
    return base.AfterCombatEnd(room);
  }

  public override Task AfterRemoved() {
    CancelLoop();
    return base.AfterRemoved();
  }

  private async Task RunMinuteLoop(CancellationToken ct) {
    try {
      while (!ct.IsCancellationRequested) {
        await LinkuraCmd.WaitRealSeconds(1f, ct);
        if (ct.IsCancellationRequested) break;
        if (Owner.Creature.CombatState?.CurrentSide != CombatSide.Player) continue;
        if (++RuriMeguSecondCount >= 60) {
          RuriMeguSecondCount = 0;
          Flash();
          await LinkuraCmd.TriggerAutoBurst(Owner, new BlockingPlayerChoiceContext());
        }
      }
    } catch (OperationCanceledException) {
      // Expected when turn ends or combat ends.
    }
  }

}
