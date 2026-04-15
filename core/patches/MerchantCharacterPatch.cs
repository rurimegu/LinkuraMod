using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using RuriMegu.Core.Characters;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Patches;

/// <summary>
/// After <see cref="NMerchantRoom._Ready"/> builds the player visuals, remap the
/// hard-coded "relaxed_loop" animation to the Linkura character's equivalent for any
/// player using a <see cref="LinkuraCharacterModel"/>.
/// </summary>
[HarmonyPatch(typeof(NMerchantRoom), nameof(NMerchantRoom._Ready))]
public static class MerchantCharacterPatch {
  private static readonly FieldInfo _playersField =
    typeof(NMerchantRoom).GetField("_players", BindingFlags.NonPublic | BindingFlags.Instance);

  [HarmonyPostfix]
  public static void Postfix(NMerchantRoom __instance) {
    var players = _playersField?.GetValue(__instance) as List<Player>;
    if (players == null) return;

    IReadOnlyList<NMerchantCharacter> visuals = __instance.PlayerVisuals;
    for (int i = 0; i < players.Count && i < visuals.Count; i++) {
      if (players[i].Character is not LinkuraCharacterModel linkuraChara) continue;

      string mappedAnim = linkuraChara.GetMappedAnimation(LinkuraAnimation.VANILLA_ANIM_RELAXED_LOOP);
      visuals[i].PlayAnimation(mappedAnim, loop: true);
    }
  }
}
