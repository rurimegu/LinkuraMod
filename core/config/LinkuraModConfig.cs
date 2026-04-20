using System.Collections.Generic;
using BaseLib.Config;
using BaseLib.Config.UI;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Runs;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Config;

public class LinkuraModConfig : SimpleModConfig {
  /// <summary>
  /// The selected spine skin name for Kaho. Empty string or BUILTIN_SKIN_LABEL means use built-in.
  /// </summary>
  [ConfigHideInUI]
  public static string KahoSkin { get; set; } = SpineSkinLoader.BUILTIN_SKIN_LABEL;

  [ConfigSection("Visual")]
  [ConfigSlider(0.1f, 5.0f, 0.1f)]
  public static float HeartMinScale { get; set; } = 1.0f;

  [ConfigSlider(0.1f, 5.0f, 0.1f)]
  public static float HeartMaxScale { get; set; } = 2.0f;

  [ConfigSlider(10, 200, 1)]
  public static int MaxFloatingHearts { get; set; } = 99;

  private class ValidationLabel(Label label) {
    private readonly Label _label = label;

    public Label Label => _label;

    public void SetSuccess(string text) {
      _label.Text = text;
      _label.AddThemeColorOverride("font_color", Colors.Green);
      _label.Show();
    }

    public void SetError(string text) {
      _label.Text = text;
      _label.AddThemeColorOverride("font_color", Colors.Crimson);
      _label.Show();
    }

    public void Hide() {
      _label.Hide();
    }

    public void Clear() {
      _label.Text = "";
      _label.Hide();
    }
  }

  private MegaSprite _previewSprite;
  private ValidationLabel _validationWarning;

  public override void SetupConfigUI(Control optionContainer) {
    LinkuraMod.Logger.Info("[LinkuraModConfig] Setting up config UI...");

    // Call base to add auto-generated controls (sliders)
    base.SetupConfigUI(optionContainer);

    _validationWarning = new ValidationLabel(new Label() {
      HorizontalAlignment = HorizontalAlignment.Center,
      CustomMinimumSize = new Vector2(0, 30),
      AutowrapMode = TextServer.AutowrapMode.WordSmart,
      Visible = false,
    });

    // Add "Skin" section header manually since KahoSkin is hidden in auto UI
    optionContainer.AddChild(CreateSectionHeader(GetLabelText("Skin")));

    var previewContainer = CreatePreviewControl();
    optionContainer.AddChild(previewContainer);
    optionContainer.AddChild(CreateKahoSkinRow());
    optionContainer.AddChild(_validationWarning.Label);
    optionContainer.AddChild(CreateDividerControl());
    AddRestoreDefaultsButton(optionContainer);
    SetupFocusNeighbors(optionContainer);

    UpdatePreviewSkin(KahoSkin);
  }

  private NConfigOptionRow CreateKahoSkinRow() {
    var label = CreateRawLabelControl(GetLabelText("KahoSkin"), 28);
    var dropdown = BuildSkinOptionButton();
    var row = new NConfigOptionRow(ModPrefix, "KahoSkin", label, dropdown);
    return row;
  }

  private OptionButton BuildSkinOptionButton() {
    var skins = SpineSkinLoader.GetAvailableSkins();

    var btn = new OptionButton();
    btn.SizeFlagsHorizontal = Control.SizeFlags.ShrinkEnd;
    btn.SizeFlagsVertical = Control.SizeFlags.Fill;
    btn.CustomMinimumSize = new Vector2(324, 64);

    for (int i = 0; i < skins.Count; i++) {
      btn.AddItem(skins[i].DisplayLabel, i);
    }

    // Set current selection
    int selectedIndex = 0;
    for (int i = 0; i < skins.Count; i++) {
      if (skins[i].FolderName == KahoSkin) {
        selectedIndex = i;
        break;
      }
    }
    btn.Selected = selectedIndex;

    btn.ItemSelected += (index) => {
      if (index >= 0 && index < skins.Count) {
        KahoSkin = skins[(int)index].FolderName;
        UpdatePreviewSkin(KahoSkin);
        Changed();
        SaveDebounced();

        if (RunManager.Instance?.NetService?.IsConnected == true) {
          RunManager.Instance.NetService.SendMessage(LinkuraNetworkState.Create());
        }
      }
    };

    return btn;
  }

  private SubViewportContainer CreatePreviewControl() {
    var container = new SubViewportContainer {
      CustomMinimumSize = new Vector2(600, 350),
      SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter,
    };

    var viewport = new SubViewport {
      TransparentBg = true,
      Size = new Vector2I(600, 350)
    };
    container.AddChild(viewport);

    var visualsScene = ResourceLoader.Load<PackedScene>("res://linkuramod/scenes/kaho/character_visuals.tscn").Instantiate<Node2D>();
    visualsScene.Position = new Vector2(300, 300);
    viewport.AddChild(visualsScene);

    _previewSprite = new MegaSprite(Variant.From(visualsScene.GetNode<Node2D>("%Visuals")));

    return container;
  }

  private void UpdatePreviewSkin(string skinName) {
    if (_previewSprite == null) return;

    MegaSkeletonDataResource data = null;
    if (skinName == SpineSkinLoader.BUILTIN_SKIN_LABEL || string.IsNullOrEmpty(skinName)) {
      var defaultDataResObj = ResourceLoader.Load("res://linkuramod/spines/kaho/kaho.tres");
      if (defaultDataResObj != null) {
        data = new MegaSkeletonDataResource(Variant.From(defaultDataResObj));
      }
    } else {
      data = SpineSkinLoader.LoadSkin(skinName);
    }

    if (data != null) {
      _previewSprite.SetSkeletonDataRes(data);
      ValidateSkinAnimations(data, skinName);
      var idleAnim = RuriMegu.Core.Characters.LinkuraCharacterModel.MAPPED_ANIMATIONS[LinkuraAnimation.VANILLA_ANIM_IDLE];
      if (data.FindAnimation(idleAnim) != null) {
        _previewSprite.GetAnimationState().SetAnimation(idleAnim, true, 0);
      }
    } else {
      _validationWarning.SetError($"Failed to load skeleton data for '{skinName}'.");
    }
  }

  private void ValidateSkinAnimations(MegaSkeletonDataResource data, string skinName) {
    var missing = new List<string>();
    foreach (var anim in RuriMegu.Core.Characters.LinkuraCharacterModel.MAPPED_ANIMATIONS.Values) {
      if (data.FindAnimation(anim) == null) {
        missing.Add(anim);
      }
    }

    if (missing.Count > 0) {
      _validationWarning.SetError($"Skin '{skinName}' missing animations:\n{string.Join(", ", missing)}");
    } else {
      _validationWarning.SetSuccess($"Skin '{skinName}' loaded successfully.");
    }
  }
}
