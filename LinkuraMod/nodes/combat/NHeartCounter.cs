using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using RuriMegu.Core.Config;
using RuriMegu.Core.Utils;

namespace RuriMegu.Nodes.Combat;

/// <summary>
/// Displays Hinoshita Kaho's "Hearts" counter — a secondary resource
/// that mirrors the Regent's star counter in layout, but with Kaho's
/// golden-pink Love Live theme.
/// </summary>
public partial class NHeartCounter : Control {
  private Player _player;
  private RichTextLabel _label = null!;
  private TextureRect _layer1 = null!;
  private Control _fillClip = null!;
  private TextureRect _layer2 = null!;
  private Subscription _heartsChangedSubscription;
  private Subscription _maxHeartsChangedSubscription;

  // ──────────────────────────────────────────────────────────────
  // Heart particle config (adjust freely)
  // ──────────────────────────────────────────────────────────────

  [Export] public float HeartMinX { get; set; } = 0.2f;
  [Export] public float HeartMaxX { get; set; } = 0.8f;
  [Export] public float HeartMinY { get; set; } = 0.1f;
  [Export] public float HeartMaxY { get; set; } = 0.6f;

  [Export] public float CollectDuration { get; set; } = 0.4f;
  [Export] public float GlowIntensity { get; set; } = 1.9f;
  [Export] public float GlowBaseAlpha { get; set; } = 0.4f;

  private Control _heartParticleLayer = null!;
  private Texture2D _glowingHeartTexture = null!;
  private Shader _glowShader = null!;
  private readonly List<FloatingHeart> _floatingHearts = new();
  private Subscription _collectSubscription;

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
    LinkuraMod.Logger.Info("[NHeartCounter] _Ready");
    _label = GetNode<RichTextLabel>("%HeartLabel");
    _layer1 = GetNode<TextureRect>("Icon/Layer1");
    _fillClip = GetNode<Control>("Icon/Layer1/FillClip");
    _layer2 = GetNode<TextureRect>("Icon/Layer1/FillClip/Layer2");
    _glowingHeartTexture = GD.Load<Texture2D>("res://LinkuraMod/images/charui/kaho/heart_glowing.png");
    _glowShader = GD.Load<Shader>("res://LinkuraMod/shaders/heart_glow.gdshader");
    Visible = false;

    // Detach HeartParticles from the energy counter's scaled hierarchy
    // (KahoEnergyCounter * HeartCounter both carry scale factors) and
    // re-attach it as a child of NCombatUi — a full-viewport, unscaled
    // Control that sits above scene content but below root-level modals.
    // Hearts then use plain viewport-space coordinates with no conversion.
    _heartParticleLayer = GetNode<Control>("HeartParticles");
    RemoveChild(_heartParticleLayer);
    // Walk up: NHeartCounter → KahoEnergyCounter → EnergyCounterContainer → NCombatUi
    var combatUi = GetParent()?.GetParent()?.GetParent();
    combatUi?.AddChild(_heartParticleLayer);

    // Register FloatingHeart with the node pool if not already done.
    FloatingHeartPool.EnsureInitialized();
  }

  public override void _ExitTree() {
    _heartsChangedSubscription?.Dispose();
    _heartsChangedSubscription = null;

    _maxHeartsChangedSubscription?.Dispose();
    _maxHeartsChangedSubscription = null;
    _collectSubscription?.Dispose();
    _collectSubscription = null;

    if (CombatManager.Instance != null)
      CombatManager.Instance.CombatEnded -= OnCombatEnded;

    FreeAllHearts();
  }

  public override void _Process(double delta) {
    if (_player is null) return;

    _lerpedHearts = MathHelper.SmoothDamp(
      _lerpedHearts, _targetHearts, ref _heartsVelocity, 0.1f, (float)delta);
    _lerpedMaxHearts = MathHelper.SmoothDamp(
      _lerpedMaxHearts, _targetMaxHearts, ref _maxHeartsVelocity, 0.1f, (float)delta);
    UpdateFill(_lerpedHearts, _lerpedMaxHearts);
    OnHeartsChanged(Mathf.RoundToInt(_lerpedHearts), Mathf.RoundToInt(_lerpedMaxHearts));
  }

  // ──────────────────────────────────────────────────────────────
  // Public API
  // ──────────────────────────────────────────────────────────────

  public void Initialize(Player player) {
    LinkuraMod.Logger.Info("[NHeartCounter] Initializing heart counter for player: " + player.NetId);
    _heartsChangedSubscription?.Dispose();
    _maxHeartsChangedSubscription?.Dispose();

    _player = player;
    foreach (var h in _floatingHearts.ToArray()) h.Dismiss();
    _floatingHearts.Clear();
    _targetHearts = HeartsState.GetHearts(player);
    _targetMaxHearts = HeartsState.GetMaxHearts(player);
    _lerpedHearts = _targetHearts;
    _lerpedMaxHearts = _targetMaxHearts;
    _heartsVelocity = 0f;
    _maxHeartsVelocity = 0f;

    _heartsChangedSubscription = Events.HeartsChanged.SubscribeLate(OnHeartsStateChanged);
    _maxHeartsChangedSubscription = Events.MaxHeartsChanged.SubscribeLate(OnMaxHeartsStateChanged);
    _collectSubscription = Events.CollectVisual.SubscribeLate(OnCollectVisualEvent);

    if (CombatManager.Instance != null)
      CombatManager.Instance.CombatEnded += OnCombatEnded;

    UpdateFill(_lerpedHearts, _lerpedMaxHearts);
    OnHeartsChanged(_targetHearts, _targetMaxHearts);
    RefreshVisibility();
  }

  // ──────────────────────────────────────────────────────────────
  // Helpers
  // ──────────────────────────────────────────────────────────────

  private Task OnHeartsStateChanged(Events.HeartsChangedEvent evt) {
    if (_player is null || evt.Player != _player) {
      return Task.CompletedTask;
    }

    _targetHearts = evt.NewHearts;
    _targetMaxHearts = evt.MaxHearts;

    int targetVisualHearts = Math.Min(evt.NewHearts, LinkuraModConfig.Settings.MaxFloatingHearts);
    int diff = targetVisualHearts - _floatingHearts.Count;

    if (diff > 0) {
      SpawnFloatingHearts(diff);
    } else if (diff < 0) {
      DismissFloatingHearts(-diff);
    }
    return Task.CompletedTask;
  }

  private Task OnMaxHeartsStateChanged(Events.MaxHeartsChangedEvent evt) {
    if (_player is null || evt.Player != _player) {
      return Task.CompletedTask;
    }

    _targetMaxHearts = evt.NewMaxHearts;
    _targetHearts = evt.Hearts;
    return Task.CompletedTask;
  }

  private void OnHeartsChanged(int newHearts, int newMaxHearts) {
    if (newMaxHearts >= 100 || newHearts >= 100)
      SetLabelText($"{newHearts}/\n{newMaxHearts}");
    else
      SetLabelText($"{newHearts}/{newMaxHearts}");
    RefreshVisibility();
  }

  private void UpdateFill(float hearts, float maxHearts) {
    var size = _layer1.Size;
    if (size.Y <= 0f) return;
    float fraction = maxHearts > 0f ? Mathf.Clamp(hearts / maxHearts, 0f, 1f) : 0f;
    float fillHeight = size.Y * fraction;
    float emptyHeight = size.Y - fillHeight;
    _fillClip.Position = new Vector2(0f, emptyHeight);
    _fillClip.Size = new Vector2(size.X, fillHeight);
    _layer2.Position = new Vector2(0f, -emptyHeight);
    _layer2.Size = size;
  }

  private void SetLabelText(string text) {
    if (_label.Text == text) return;
    _label.Text = text;
  }

  private void RefreshVisibility() {
    if (_player is null) { Visible = false; return; }
    Visible = true;
  }

  private void SpawnFloatingHearts(int count) {
    var viewportSize = GetViewportRect().Size;
    for (int i = 0; i < count; i++) SpawnOneHeart(viewportSize);
  }

  private void DismissFloatingHearts(int count) {
    int toDismiss = Math.Min(count, _floatingHearts.Count);
    for (int i = 0; i < toDismiss; i++) {
      int lastIndex = _floatingHearts.Count - 1;
      var heart = _floatingHearts[lastIndex];
      heart.Dismiss();
      _floatingHearts.RemoveAt(lastIndex);
    }
  }

  private void SpawnOneHeart(Vector2 viewportSize) {
    float scale = (float)GD.RandRange(LinkuraModConfig.Settings.HeartMinScale, LinkuraModConfig.Settings.HeartMaxScale);
    var size = _glowingHeartTexture.GetSize() * scale;
    float x = (float)GD.RandRange(HeartMinX, HeartMaxX) * viewportSize.X - size.X / 2f;
    float y = (float)GD.RandRange(HeartMinY, HeartMaxY) * viewportSize.Y - size.Y / 2f;

    var heart = FloatingHeartPool.Get();
    heart.Configure(
      heartTexture: _glowingHeartTexture,
      glowShader: _glowShader,
      heartScale: scale,
      glowIntensity: GlowIntensity,
      glowBaseAlpha: GlowBaseAlpha,
      collectDuration: CollectDuration,
      position: new Vector2(x, y));
    _floatingHearts.Add(heart);
    heart.TreeExited += () => _floatingHearts.Remove(heart);
    _heartParticleLayer.AddChild(heart);
    heart.StartSpawnAnimation();
  }

  private async Task OnCollectVisualEvent(Events.CollectVisualEvent evt) {
    if (_player is null || evt.Player != _player) return;
    await CollectAllHearts(GetTargetPositions(evt.Targets));
  }

  private async Task CollectAllHearts(Vector2[] targetPositions) {
    var snapshot = _floatingHearts.ToArray();
    _floatingHearts.Clear();
    if (targetPositions.Length == 0) {
      foreach (var h in snapshot) h.Dismiss();
      return;
    }
    var tasks = new Task[snapshot.Length];
    for (int i = 0; i < snapshot.Length; i++) {
      tasks[i] = snapshot[i].Collect(targetPositions[i % targetPositions.Length]);
    }
    await Task.WhenAll(tasks);
  }

  private Vector2[] GetTargetPositions(IReadOnlyList<Creature> targets) {
    if (targets is null || targets.Count == 0) return [];
    var positions = new List<Vector2>(targets.Count);
    foreach (var target in targets) {
      var node = NCombatRoom.Instance?.GetCreatureNode(target);
      if (node != null) positions.Add(node.VfxSpawnPosition);
    }
    return [.. positions];
  }

  private void OnCombatEnded(CombatRoom _) => FreeAllHearts();

  private void FreeAllHearts() {
    foreach (var h in _floatingHearts.ToArray()) {
      if (GodotObject.IsInstanceValid(h)) FloatingHeartPool.Free(h);
    }
    _floatingHearts.Clear();
  }
}
