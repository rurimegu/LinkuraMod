using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using RuriMegu.Core.Config;
using RuriMegu.Core.Utils;

namespace RuriMegu.Nodes.Shop;

public partial class NLinkuraMerchantCharacter : NMerchantCharacter {
  public override void _Ready() {
    SpineSkinLoader.SwapSkin(LinkuraModConfig.KahoSkin, new MegaSprite(GetNode<Node2D>("SpineSprite")));
    base._Ready();
  }
}
