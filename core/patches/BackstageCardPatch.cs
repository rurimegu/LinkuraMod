#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using RuriMegu.Core.Cards;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Patches;

[HarmonyPatch(typeof(NHandCardHolder), "SubscribeToEvents")]
public static class BackstageCardSubscribePatch {
  // ── Tunable ──────────────────────────────────────────────────────────────
  // Shader shape (core_half, glow_half, glow_strength) in backstage_glint.gdshader
  private const float GLINT_DURATION = 0.55f;
  private const float GLINT_START = -0.15f;
  private const float GLINT_END = 1.15f;
  // ─────────────────────────────────────────────────────────────────────────

  internal static readonly Dictionary<InHandTriggerCard, NHandCardHolder> Holders = new();

  private static ShaderMaterial? _glintTemplate;
  private static ShaderMaterial GlintTemplate => _glintTemplate ??= new ShaderMaterial {
    Shader = ResourceLoader.Load<Shader>("res://linkuramod/shaders/backstage_glint.gdshader")
  };

  static BackstageCardSubscribePatch() {
    Events.TriggerBackstage.SubscribeLate(OnTriggerBackstage);
  }

  private static async Task OnTriggerBackstage(Events.TriggerBackstageEvent ev) {
    if (ev.Source is not InHandTriggerCard card) return;
    if (!Holders.TryGetValue(card, out var holder)) return;
    if (!GodotObject.IsInstanceValid(holder)) return;

    SfxCmd.Play(FmodSfx.relicFlashGeneral);
    holder.Flash();
    _ = DoGlintAsync(holder);
  }

  [HarmonyPostfix]
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

[HarmonyPatch(typeof(NHandCardHolder), "UnsubscribeFromEvents")]
public static class BackstageCardUnsubscribePatch {
  [HarmonyPostfix]
  public static void Postfix(CardModel card) {
    if (card is not InHandTriggerCard trigger) return;
    BackstageCardSubscribePatch.Holders.Remove(trigger);
  }
}
