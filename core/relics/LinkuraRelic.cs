using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Rooms;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Relics;

/// <summary>
/// Base class for all Linkura relics.
/// Provides subscription tracking with self-managed lifecycle:
/// subscriptions are initialized when combat starts or when the relic is obtained mid-combat,
/// and disposed automatically at combat end or relic removal.
/// </summary>
public abstract class LinkuraRelic : CustomRelicModel {
  public abstract string CharacterId { get; }
  public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath(CharacterId);
  protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath(CharacterId);
  protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath(CharacterId);

  private List<Subscription> _subs = [];
  private bool _subscriptionsInitialized;

  protected override void DeepCloneFields() {
    base.DeepCloneFields();
    _subs = [];
    _subscriptionsInitialized = false;
  }

  /// <summary>
  /// Override to set up subscriptions via TrackSubscription().
  /// Called automatically at BeforeCombatStartLate and when obtained mid-combat.
  /// </summary>
  protected virtual Task InitializeSubscriptions() => Task.CompletedTask;

  /// <summary>Track a subscription for automatic cleanup.</summary>
  protected void TrackSubscription(Subscription sub) => _subs.Add(sub);

  private void DisposeAllSubscriptions() {
    foreach (var sub in _subs) sub.Dispose();
    _subs.Clear();
    _subscriptionsInitialized = false;
  }

  private async Task EnsureSubscriptionsInitialized() {
    if (_subscriptionsInitialized) return;
    _subscriptionsInitialized = true;
    await InitializeSubscriptions();
  }

  /// <summary>
  /// Initialize subscriptions for relics already held at combat start.
  /// </summary>
  public override async Task BeforeCombatStartLate() {
    await EnsureSubscriptionsInitialized();
    await base.BeforeCombatStartLate();
  }

  /// <summary>
  /// Initialize subscriptions for relics obtained mid-combat.
  /// </summary>
  public override async Task AfterObtained() {
    if (CombatManager.Instance.IsInProgress)
      await EnsureSubscriptionsInitialized();
    await base.AfterObtained();
  }

  public override Task AfterCombatEnd(CombatRoom room) {
    DisposeAllSubscriptions();
    return base.AfterCombatEnd(room);
  }

  public override Task AfterRemoved() {
    DisposeAllSubscriptions();
    return base.AfterRemoved();
  }
}
