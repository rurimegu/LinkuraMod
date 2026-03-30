using System;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Nodes.Combat;

/// <summary>
/// Displays Hinoshita Kaho's "Hearts" counter — a secondary resource
/// that mirrors the Regent's star counter in layout, but with Kaho's
/// golden-pink Love Live theme. Backed by <see cref="PlayerCombatState.Stars"/>.
/// </summary>
public partial class NHeartCounter : Control {
  private Player _player;
  private RichTextLabel _label = null!;
  private IDisposable _heartsChangedSubscription;
  private IDisposable _maxHeartsChangedSubscription;

  // Smooth-damp state for the animated label
  private int _targetHearts;
  private int _targetMaxHearts;
  private float _lerpedHearts;
  private float _lerpedMaxHearts;
  private float _heartsVelocity;
  private float _maxHeartsVelocity;

  // ──────────────────────────────────────────────────────────────
  // Godot lifecycle
  // ──────────────────────────────────────────────────────────────

  public override void _Ready() {
    _label = GetNode<RichTextLabel>("%HeartLabel");
    Visible = false;
  }

  public override void _ExitTree() {
    _heartsChangedSubscription?.Dispose();
    _heartsChangedSubscription = null;

    _maxHeartsChangedSubscription?.Dispose();
    _maxHeartsChangedSubscription = null;
  }

  public override void _Process(double delta) {
    if (_player is null) return;

    _lerpedHearts = MathHelper.SmoothDamp(
      _lerpedHearts, _targetHearts, ref _heartsVelocity, 0.1f, (float)delta);
    _lerpedMaxHearts = MathHelper.SmoothDamp(
      _lerpedMaxHearts, _targetMaxHearts, ref _maxHeartsVelocity, 0.1f, (float)delta);
    OnHeartsChanged(Mathf.RoundToInt(_lerpedHearts), Mathf.RoundToInt(_lerpedMaxHearts));
  }

  // ──────────────────────────────────────────────────────────────
  // Public API
  // ──────────────────────────────────────────────────────────────

  public void Initialize(Player player) {
    _heartsChangedSubscription?.Dispose();
    _maxHeartsChangedSubscription?.Dispose();

    _player = player;
    _targetHearts = HeartsState.GetHearts(player);
    _targetMaxHearts = HeartsState.GetMaxHearts(player);
    _lerpedHearts = _targetHearts;
    _lerpedMaxHearts = _targetMaxHearts;
    _heartsVelocity = 0f;
    _maxHeartsVelocity = 0f;

    _heartsChangedSubscription = HeartsState.SubscribeHeartsChanged(OnHeartsStateChanged);
    _maxHeartsChangedSubscription = HeartsState.SubscribeMaxHeartsChanged(OnMaxHeartsStateChanged);

    OnHeartsChanged(_targetHearts, _targetMaxHearts);
    RefreshVisibility();
  }

  // ──────────────────────────────────────────────────────────────
  // Helpers
  // ──────────────────────────────────────────────────────────────

  private void OnHeartsStateChanged(HeartsState.HeartsChangedEvent evt) {
    if (_player is null || evt.Player != _player) {
      return;
    }

    _targetHearts = evt.NewHearts;
    _targetMaxHearts = evt.MaxHearts;
  }

  private void OnMaxHeartsStateChanged(HeartsState.MaxHeartsChangedEvent evt) {
    if (_player is null || evt.Player != _player) {
      return;
    }

    _targetMaxHearts = evt.NewMaxHearts;
    _targetHearts = evt.Hearts;
  }

  private void OnHeartsChanged(int newHearts, int newMaxHearts) {
    SetLabelText($"{newHearts}/{newMaxHearts}");
    RefreshVisibility();
  }

  private void SetLabelText(string text) {
    if (_label.Text == text) return;
    _label.Text = text;
  }

  private void RefreshVisibility() {
    if (_player is null) { Visible = false; return; }
    Visible = true;
  }
}
