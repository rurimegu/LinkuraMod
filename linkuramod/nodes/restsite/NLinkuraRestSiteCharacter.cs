using System;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Random;
using RuriMegu.Core.Characters;
using RuriMegu.Core.Utils;

namespace RuriMegu.Nodes.RestSite;

public partial class NLinkuraRestSiteCharacter : NRestSiteCharacter {
  private const string CHARACTER_ID = "linkura";

  public override void _Ready() {
    base._Ready();
    if (Player.Character is not LinkuraCharacterModel linkuraChara) {
      LinkuraMod.Logger.Warn($"Unexpected character type in {nameof(NLinkuraRestSiteCharacter)}: {Player.Character?.GetType().Name}");
      return;
    }
    string animKey = Player.RunState.CurrentActIndex switch {
      0 => LinkuraAnimation.VANILLA_ANIM_REST_SITE_ACT1,
      1 => LinkuraAnimation.VANILLA_ANIM_REST_SITE_ACT2,
      2 => LinkuraAnimation.VANILLA_ANIM_REST_SITE_ACT3,
      _ => throw new InvalidOperationException("Unexpected act"),
    };
    string animationName = linkuraChara.GetMappedAnimation(animKey);
    foreach (Node2D childSpineNode in GetChildren().OfType<Node2D>().Where(n => n.GetClass() == "SpineSprite")) {
      MegaTrackEntry megaTrackEntry = new MegaSprite(childSpineNode).GetAnimationState().SetAnimation(animationName);
      megaTrackEntry?.SetTrackTime(megaTrackEntry.GetAnimationEnd() * Rng.Chaotic.NextFloat());
    }
  }
}
