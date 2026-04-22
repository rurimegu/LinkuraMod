using Godot;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using RuriMegu.Core.Config;

namespace RuriMegu.Nodes.RestSite;

public partial class NLinkuraRestSiteCharacter : NRestSiteCharacter {
  public override void _Ready() {
    ulong playerId = Player?.NetId ?? LinkuraNetwork.SINGLE_PLAYER_ID;
    LinkuraNetwork.ApplySyncedSkin(GetNode<Node2D>("SpineSprite"), playerId);
    base._Ready();
  }
}
