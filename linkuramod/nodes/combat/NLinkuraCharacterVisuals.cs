using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using RuriMegu.Core.Config;
using RuriMegu.Core.Utils;

namespace RuriMegu.Nodes.Combat;

/// <summary>
/// Attached to the character_visuals.tscn root. Extends NCreatureVisuals so BaseLib
/// does not auto-convert the scene root. Swaps the SpineSprite skeleton before
/// base._Ready() so NCreatureVisuals initialises SpineBody against the correct data.
/// </summary>
public partial class NLinkuraCharacterVisuals : NCreatureVisuals {
  public override void _Ready() {
    var skin = LinkuraModConfig.KahoSkin;
    if (!string.IsNullOrEmpty(skin) && skin != SpineSkinLoader.BUILTIN_SKIN_LABEL) {
      var data = SpineSkinLoader.LoadSkin(skin);
      if (data != null)
        new MegaSprite(GetNode<Node2D>("%Visuals")).SetSkeletonDataRes(data);
    }
    base._Ready();
  }
}
