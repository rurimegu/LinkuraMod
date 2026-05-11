using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Pooling;
using RuriMegu.Core.Utils;

namespace RuriMegu.Nodes.Combat;

/// <summary>
/// A single floating heart particle spawned when Kaho gains hearts.
/// Pop-up jump → slow exponential drift downward → persists until collected.
/// Call <see cref="Collect"/> to fly toward a target and shrink to zero,
/// or <see cref="Dismiss"/> to fade out when there are no targets.
/// <para>
/// Instances are managed via <see cref="GeneratedNodePool"/>. Always obtain
/// instances through <see cref="NodePool.Get{FloatingHeart}"/> and never call
/// <see cref="Node.QueueFree"/> directly — use
/// <see cref="GodotTreeExtensions.QueueFreeSafely"/> instead so the node is
/// returned to the pool automatically.
/// </para>
/// </summary>
public partial class FloatingHeart : Control, IPoolable {
  // ── Set before adding to the scene tree ──────────────────────
  public Texture2D HeartTexture { get; set; } = null!;
  public Shader GlowShader { get; set; } = null!;
  public float HeartScale { get; set; } = 1.0f;
  public float GlowIntensity { get; set; } = 1.9f;
  public float GlowBaseAlpha { get; set; } = 0.8f;
  public float CollectDuration { get; set; } = 0.4f;

  private Tween _settleTween;
  private bool _collecting;
  private TextureRect _textureRect;
  private ShaderMaterial _shaderMaterial;

  // ── IPoolable ────────────────────────────────────────────────

  /// <summary>Called exactly once when the pool creates a fresh instance.</summary>
  public void OnInstantiated() {
    _shaderMaterial = new ShaderMaterial();
    _textureRect = new TextureRect {
      ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
      MouseFilter = MouseFilterEnum.Ignore,
    };
    AddChild(_textureRect);
    MouseFilter = MouseFilterEnum.Ignore;
  }

  /// <summary>Called each time the node is retrieved from the pool. Configure
  /// per-spawn properties here (textures, scale, position, etc.) before adding
  /// to the tree, then call <see cref="StartSpawnAnimation"/> once in tree.</summary>
  public void OnReturnedFromPool() {
    _collecting = false;
    // Kill any leftover tween from a previous use
    _settleTween?.Kill();
    _settleTween = null;
    Scale = Vector2.One;
    Modulate = Colors.Transparent;
  }

  /// <summary>Called when the node is returned to the pool after
  /// <see cref="GodotTreeExtensions.QueueFreeSafely"/> removes it from the tree.</summary>
  public void OnFreedToPool() {
    // Nothing extra needed — signals are disconnected by GeneratedNodePool.
  }

  // ── Pool-aware setup ─────────────────────────────────────────

  /// <summary>
  /// Apply per-spawn visual configuration. Call this after
  /// <c>NodePool.Get&lt;FloatingHeart&gt;()</c> and before adding to the scene tree.
  /// </summary>
  public void Configure(
      Texture2D heartTexture,
      Shader glowShader,
      float heartScale,
      float glowIntensity,
      float glowBaseAlpha,
      float collectDuration,
      Vector2 position) {
    HeartTexture = heartTexture;
    GlowShader = glowShader;
    HeartScale = heartScale;
    GlowIntensity = glowIntensity;
    GlowBaseAlpha = glowBaseAlpha;
    CollectDuration = collectDuration;
    Position = position;

    var texSize = (Vector2)HeartTexture.GetSize();
    var size = texSize * HeartScale;
    Size = size;
    PivotOffset = size / 2f;

    _shaderMaterial.Shader = GlowShader;
    _shaderMaterial.SetShaderParameter("glow_high", GlowIntensity);
    _shaderMaterial.SetShaderParameter("base_alpha", GlowBaseAlpha);
    _textureRect.Texture = HeartTexture;
    _textureRect.Size = size;
    _textureRect.Material = _shaderMaterial;
  }

  /// <summary>
  /// Begin the pop-up spawn animation. Call this once the node has been added
  /// to the scene tree. If <see cref="Collect"/> was already called before
  /// entering the tree, this is a no-op so the collect tween takes priority.
  /// </summary>
  public void StartSpawnAnimation() {
    if (_collecting) return;

    float startY = Position.Y;
    float dropDist = (float)GD.RandRange(60.0, 150.0);

    _settleTween = CreateTween();
    _settleTween.TweenProperty(this, "modulate:a", 1.0f, 0.2f);
    _settleTween.Parallel()
      .TweenProperty(this, "position:y", startY - 25f, 0.12f)
      .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
    _settleTween
      .TweenProperty(this, "position:y", startY + dropDist, 8.0f)
      .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
  }

  // ── Public gameplay API ──────────────────────────────────────

  /// <summary>Fly toward <paramref name="targetPos"/> (screen space), shrink to zero, then return to pool.</summary>
  public Task Collect(Vector2 targetPos) {
    if (_collecting) return Task.CompletedTask;
    _collecting = true;
    _settleTween?.Kill();
    Modulate = Colors.White; // ensure visible even if spawn fade-in hadn't completed

    var tcs = new TaskCompletionSource();
    var tween = CreateTween();
    tween.TweenProperty(this, "position", targetPos - Size / 2f, CollectDuration)
      .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quad);
    tween.Parallel()
      .TweenProperty(this, "scale", Vector2.Zero, CollectDuration)
      .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quad);
    tween.TweenCallback(Callable.From(() => {
      FloatingHeartPool.Free(this);
      tcs.SetResult();
    }));
    return tcs.Task;
  }

  /// <summary>Fade out and return to pool (used when collection has no targets).</summary>
  public void Dismiss() {
    if (_collecting) return;
    _collecting = true;
    _settleTween?.Kill();

    var tween = CreateTween();
    tween.TweenProperty(this, "modulate:a", 0.0f, 0.5f);
    tween.TweenCallback(Callable.From(() => FloatingHeartPool.Free(this)));
  }
}
