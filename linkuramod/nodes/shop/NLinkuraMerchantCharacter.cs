using System.Collections.Generic;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using RuriMegu.Core.Characters;
using RuriMegu.Core.Utils;

namespace RuriMegu.Nodes.Shop;

public partial class NLinkuraMerchantCharacter : NMerchantCharacter {

  public override void _Ready() {
    base._Ready();
    string animName = LinkuraCharacterModel.MAPPED_ANIMATIONS.GetValueOrDefault(LinkuraAnimation.VANILLA_ANIM_RELAXED_LOOP, "quest_dance_general00");
    LinkuraMod.Logger.Info($"Playing merchant animation: {animName}");
    PlayAnimation(animName, true);
  }
}
