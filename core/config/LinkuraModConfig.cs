using System;
using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using RuriMegu.Core.Characters;
using RuriMegu.Core.Utils;
using STS2RitsuLib;
using STS2RitsuLib.Data;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils.Persistence;

namespace RuriMegu.Core.Config;

public static class LinkuraModConfig {
  private const string SETTINGS_KEY = "linkura_settings";

  private static string _modId;
  private static IModSettingsValueBinding<string> _kahoSkinBinding;
  private static IModSettingsValueBinding<double> _heartMinBinding;
  private static IModSettingsValueBinding<double> _heartMaxBinding;
  private static IModSettingsValueBinding<int> _maxHeartsBinding;

  public static LinkuraSettingsModel Settings =>
    ModDataStore.For(_modId).Get<LinkuraSettingsModel>(SETTINGS_KEY);

  private static ModSettingsText SettingsText(string displayText) {
    string key = "LINKURA_MOD_" + displayText.Replace(' ', '_').ToUpperInvariant() + ".title";
    return ModSettingsText.LocString("settings_ui", key, displayText);
  }

  public static void RegisterSettings(string modId) {
    _modId = modId;
    using (RitsuLibFramework.BeginModDataRegistration(modId)) {
      ModDataStore.For(modId).Register<LinkuraSettingsModel>(
        SETTINGS_KEY, SETTINGS_KEY, SaveScope.Global,
        defaultFactory: () => new LinkuraSettingsModel());
    }

    _kahoSkinBinding = ModSettingsBindings.Global<LinkuraSettingsModel, string>(
      modId, SETTINGS_KEY, m => m.KahoSkin, (m, v) => m.KahoSkin = v);
    _heartMinBinding = ModSettingsBindings.Global<LinkuraSettingsModel, double>(
      modId, SETTINGS_KEY, m => m.HeartMinScale, (m, v) => m.HeartMinScale = v);
    _heartMaxBinding = ModSettingsBindings.Global<LinkuraSettingsModel, double>(
      modId, SETTINGS_KEY, m => m.HeartMaxScale, (m, v) => m.HeartMaxScale = v);
    _maxHeartsBinding = ModSettingsBindings.Global<LinkuraSettingsModel, int>(
      modId, SETTINGS_KEY, m => m.MaxFloatingHearts, (m, v) => m.MaxFloatingHearts = v);

    List<SkinEntry> skins = SpineSkinLoader.GetAvailableSkins();
    var skinChoiceOptions = new List<ModSettingsChoiceOption<string>>();
    foreach (var skin in skins) {
      skinChoiceOptions.Add(new ModSettingsChoiceOption<string>(skin.FolderName, ModSettingsText.Literal(skin.DisplayLabel)));
    }

    RitsuLibFramework.RegisterModSettings(modId, page => page
      .WithModDisplayName(ModSettingsText.LocString("settings_ui", "LINKURA_MOD.mod_title", "LinkuraMod"))
      .WithTitle(ModSettingsText.LocString("settings_ui", "LINKURA_MOD.settings_title", "LinkuraMod Settings"))
      .AddSection("skin", section => section
        .WithTitle(SettingsText("Skin"))
        .AddCustom(
          "kaho_skin_preview",
          SettingsText("Kaho Preview"),
          _ => CreateKahoSkinPreview())
        .AddChoice(
          "kaho_skin",
          SettingsText("Kaho Skin"),
          _kahoSkinBinding,
          skinChoiceOptions,
          presentation: ModSettingsChoicePresentation.Dropdown))
      .AddSection("visual", section => section
        .WithTitle(SettingsText("Visual"))
        .AddSlider(
          "heart_min_scale",
          SettingsText("Heart Min Scale"),
          _heartMinBinding,
          minValue: 0.1,
          maxValue: 5.0,
          step: 0.1,
          valueFormatter: v => $"{v:F1}x")
        .AddSlider(
          "heart_max_scale",
          SettingsText("Heart Max Scale"),
          _heartMaxBinding,
          minValue: 0.1,
          maxValue: 5.0,
          step: 0.1,
          valueFormatter: v => $"{v:F1}x")
        .AddIntSlider(
          "max_floating_hearts",
          SettingsText("Max Floating Hearts"),
          _maxHeartsBinding,
          minValue: 10,
          maxValue: 200)));
  }

  private static Control CreateKahoSkinPreview() {
    var root = new VBoxContainer {
      SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
    };

    var viewportContainer = new SubViewportContainer {
      CustomMinimumSize = new Vector2(600, 350),
      SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter,
      StretchShrink = 1,
    };
    var viewport = new SubViewport {
      TransparentBg = true,
      Size = new Vector2I(600, 350),
    };
    viewportContainer.AddChild(viewport);
    root.AddChild(viewportContainer);

    var validationLabel = new ValidationLabel(new Label {
      SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
      AutowrapMode = TextServer.AutowrapMode.WordSmart,
    });
    root.AddChild(validationLabel.Node);

    var scene = ResourceLoader.Load<PackedScene>("res://LinkuraMod/scenes/kaho/character_visuals.tscn");
    if (scene == null) {
      validationLabel.SetError("Failed to load character_visuals.tscn");
      return root;
    }

    var instance = scene.Instantiate<Node2D>();
    instance.Position = new Vector2(300, 300);
    viewport.AddChild(instance);

    var spritesNode = instance.GetNode<Node2D>("%Visuals");
    var sprite = new MegaSprite(Variant.From(spritesNode));

    ModSettingsBindingWriteEvents.SubscribeValueWrittenWhileNodeAlive(root, binding => {
      if (!ReferenceEquals(binding, _kahoSkinBinding)) return;
      UpdatePreviewSkin(sprite, validationLabel, _kahoSkinBinding.Read());
    });

    root.TreeEntered += () => {
      Callable.From(() => {
        if (!GodotObject.IsInstanceValid(root)) return;
        UpdatePreviewSkin(sprite, validationLabel, _kahoSkinBinding.Read());
      }).CallDeferred();
    };

    return root;
  }

  private static void UpdatePreviewSkin(MegaSprite sprite, ValidationLabel label, string skinName) {
    MegaSkeletonDataResource data;
    if (string.IsNullOrEmpty(skinName) || skinName == SpineSkinLoader.BUILTIN_SKIN_LABEL) {
      var raw = ResourceLoader.Load("res://LinkuraMod/spines/kaho/kaho.tres");
      data = raw != null ? new MegaSkeletonDataResource(Variant.From(raw)) : null;
    } else {
      data = SpineSkinLoader.LoadSkin(skinName);
    }

    if (data == null) {
      label.SetError($"Failed to load skeleton data for '{skinName}'.");
      return;
    }

    sprite.SetSkeletonDataRes(data);
    ValidateSkinAnimations(data, skinName, label);

    string idleAnim = LinkuraAnimation.MAPPED_ANIMATIONS.GetValueOrDefault(LinkuraAnimation.VANILLA_ANIM_IDLE)
      ?? "quest_dance_general00";
    sprite.GetAnimationState().SetAnimation(idleAnim, true);
  }

  private static void ValidateSkinAnimations(MegaSkeletonDataResource data, string skinName, ValidationLabel label) {
    var missing = new List<string>();
    foreach (string anim in LinkuraAnimation.MAPPED_ANIMATIONS.Values) {
      if (data.FindAnimation(anim) == null)
        missing.Add(anim);
    }

    if (missing.Count > 0) {
      label.SetError($"'{skinName}' missing animations: {string.Join(", ", missing)}");
    } else {
      label.SetSuccess($"'{skinName}' animations OK.");
    }
  }

  private sealed class ValidationLabel {
    public Label Node { get; }

    public ValidationLabel(Label node) {
      Node = node;
      Node.Hide();
    }

    public void SetSuccess(string message) {
      Node.Text = message;
      Node.AddThemeColorOverride("font_color", new Color(0.2f, 0.8f, 0.2f));
      Node.Show();
    }

    public void SetError(string message) {
      Node.Text = message;
      Node.AddThemeColorOverride("font_color", new Color(0.86f, 0.08f, 0.24f));
      Node.Show();
    }

    public void Clear() {
      Node.Text = string.Empty;
      Node.Hide();
    }
  }

  public sealed class LinkuraSettingsModel {
    private double _heartMinScale = 1.0;
    private double _heartMaxScale = 2.0;

    public string KahoSkin { get; set; } = SpineSkinLoader.BUILTIN_SKIN_LABEL;

    public double HeartMinScale {
      get => _heartMinScale;
      set => _heartMinScale = Math.Min(value, _heartMaxScale);
    }

    public double HeartMaxScale {
      get => _heartMaxScale;
      set => _heartMaxScale = Math.Max(value, _heartMinScale);
    }

    public int MaxFloatingHearts { get; set; } = 99;
  }
}
