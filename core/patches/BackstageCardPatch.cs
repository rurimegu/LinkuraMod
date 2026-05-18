#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using RuriMegu.Core.Cards;
using RuriMegu.Core.Utils;
using STS2RitsuLib.Patching.Core;
using STS2RitsuLib.Patching.Models;

namespace RuriMegu.Core.Patches;

public class BackstageCardPatch : IModPatches {
  public static void AddTo(ModPatcher patcher) {
    patcher.RegisterPatch<BackstageCardSubscribePatch>();
    patcher.RegisterPatch<BackstageCardUnsubscribePatch>();
  }
}

public class BackstageCardSubscribePatch : IPatchMethod {
  public static string PatchId => "backstage_card_subscribe";
  public static string Description => "Tracks NHandCardHolder per InHandTriggerCard and plays glint on backstage trigger";
  public static bool IsCritical => false;

  private const float GLINT_DURATION = 0.55f;
  private const float GLINT_START = -0.15f;
  private const float GLINT_END = 1.15f;

  internal static readonly Dictionary<InHandTriggerCard, NHandCardHolder> Holders = new();

  private static ShaderMaterial? _glintTemplate;
  private static ShaderMaterial GlintTemplate => _glintTemplate ??= new ShaderMaterial {
    Shader = ResourceLoader.Load<Shader>("res://LinkuraMod/shaders/backstage_glint.gdshader")
  };

  static BackstageCardSubscribePatch() {
    Events.TriggerBackstage.SubscribeLate(OnTriggerBackstage);
  }

  private static Task OnTriggerBackstage(Events.TriggerBackstageEvent ev) {
    if (ev.Source is not InHandTriggerCard card) return Task.CompletedTask;
    if (!Holders.TryGetValue(card, out var holder)) return Task.CompletedTask;
    if (!GodotObject.IsInstanceValid(holder)) return Task.CompletedTask;

    SfxCmd.Play(FmodSfx.relicFlashGeneral);
    holder.Flash();
    _ = DoGlintAsync(holder);
    return Task.CompletedTask;
  }

  public static ModPatchTarget[] GetTargets() =>
    [new(typeof(NHandCardHolder), "SubscribeToEvents")];

  public static void Postfix(NHandCardHolder __instance, CardModel card) {
    if (card is not InHandTriggerCard trigger) return;
    Holders[trigger] = __instance;
  }

  private static async Task DoGlintAsync(NHandCardHolder holder) {
    await holder.ToSignal(holder.GetTree(), SceneTree.SignalName.ProcessFrame);
    if (!GodotObject.IsInstanceValid(holder)) return;

    var mat = (ShaderMaterial)GlintTemplate.Duplicate();
    mat.SetShaderParameter("progress", GLINT_START);
    var glintRect = new ColorRect {
      MouseFilter = Control.MouseFilterEnum.Ignore,
      ZIndex = 0,
      Material = mat,
      Position = -NCard.defaultSize / 2f,
      Size = NCard.defaultSize,
    };
    holder.AddChild(glintRect);

    var tween = holder.CreateTween();
    tween.TweenMethod(
      Callable.From<Variant>(v => mat.SetShaderParameter("progress", v)),
      Variant.From(GLINT_START), Variant.From(GLINT_END), GLINT_DURATION);
    tween.TweenCallback(Callable.From(() => {
      if (GodotObject.IsInstanceValid(glintRect)) glintRect.QueueFree();
    }));
  }
}

public class BackstageCardUnsubscribePatch : IPatchMethod {
  public static string PatchId => "backstage_card_unsubscribe";
  public static string Description => "Removes NHandCardHolder from tracker when card leaves hand";
  public static bool IsCritical => false;

  public static ModPatchTarget[] GetTargets() =>
    [new(typeof(NHandCardHolder), "UnsubscribeFromEvents")];

  public static void Postfix(CardModel card) {
    if (card is not InHandTriggerCard trigger) return;
    BackstageCardSubscribePatch.Holders.Remove(trigger);
  }
}
