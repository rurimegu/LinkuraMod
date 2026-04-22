using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;
using RuriMegu.Core.Config;

namespace RuriMegu.Nodes.Combat;

/// <summary>
/// Attached to the character_visuals.tscn root. Extends NCreatureVisuals so BaseLib
/// does not auto-convert the scene root. Swaps the SpineSprite skeleton before
/// base._Ready() so NCreatureVisuals initialises SpineBody against the correct data.
/// </summary>
public partial class NLinkuraCharacterVisuals : NCreatureVisuals {
  public override void _Ready() {
    ulong playerId = GetParentOrNull<NCreature>()?.Entity?.Player?.NetId
      ?? LinkuraNetwork.SINGLE_PLAYER_ID;

    LinkuraNetwork.ApplySyncedSkin(GetNode<Node2D>("%Visuals"), playerId);
    base._Ready();
  }
}
