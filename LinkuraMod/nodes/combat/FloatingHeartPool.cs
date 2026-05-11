using System.Collections.Generic;
using Godot;

namespace RuriMegu.Nodes.Combat;

/// <summary>
/// Simple factory-based pool for <see cref="FloatingHeart"/> nodes.
/// Replaces the RitsuLib <c>GeneratedNodePool</c> which no longer exists.
/// </summary>
public static class FloatingHeartPool {
  private static readonly List<FloatingHeart> _free = new();

  public static void EnsureInitialized() {
    // Nothing to do — pool initializes lazily on first Get().
  }

  public static FloatingHeart Get() {
    FloatingHeart heart;
    if (_free.Count > 0) {
      int last = _free.Count - 1;
      heart = _free[last];
      _free.RemoveAt(last);
      heart.OnReturnedFromPool();
    } else {
      heart = new FloatingHeart();
      heart.OnInstantiated();
    }
    return heart;
  }

  public static void Free(FloatingHeart heart) {
    if (!GodotObject.IsInstanceValid(heart)) return;
    heart.OnFreedToPool();
    heart.GetParent()?.RemoveChild(heart);
    _free.Add(heart);
  }
}
