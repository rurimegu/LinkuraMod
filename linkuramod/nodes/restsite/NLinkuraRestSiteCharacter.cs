using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using RuriMegu.Core.Config;
using RuriMegu.Core.Utils;

namespace RuriMegu.Nodes.RestSite;

public partial class NLinkuraRestSiteCharacter : NRestSiteCharacter {
  public override void _Ready() {
    SpineSkinLoader.SwapSkin(LinkuraModConfig.KahoSkin, new MegaSprite(GetNode<Node2D>("SpineSprite")));
    base._Ready();
  }
}
