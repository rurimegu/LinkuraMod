using BaseLib.Config;
using BaseLib.Config.UI;
using Godot;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Config;

public class LinkuraModConfig : SimpleModConfig {
  /// <summary>
  /// The selected spine skin name for Kaho. Empty string or BUILTIN_SKIN_LABEL means use built-in.
  /// </summary>
  public static string KahoSkin { get; set; } = SpineSkinLoader.BUILTIN_SKIN_LABEL;

  public override void SetupConfigUI(Control optionContainer) {
    LinkuraMod.Logger.Info("[LinkuraModConfig] Setting up config UI...");
    optionContainer.AddChild(CreateSectionHeader("Kaho"));
    optionContainer.AddChild(CreateKahoSkinRow());
    optionContainer.AddChild(CreateDividerControl());
    AddRestoreDefaultsButton(optionContainer);
    SetupFocusNeighbors(optionContainer);
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
        Changed();
        SaveDebounced();
      }
    };

    return btn;
  }
}
