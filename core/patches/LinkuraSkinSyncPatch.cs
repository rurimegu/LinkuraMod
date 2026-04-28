using System.Collections.Generic;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using RuriMegu.Core.Utils;
using RuriMegu.Nodes.Shop;

namespace RuriMegu.Core.Config;

[HarmonyPatch]
public static class LinkuraSkinSyncPatch {
  [HarmonyPatch(typeof(NetMessageBus), MethodType.Constructor)]
  [HarmonyPostfix]
  public static void NetMessageBus_Postfix(NetMessageBus __instance) {
    __instance.RegisterMessageHandler<LinkuraNetworkState>((msg, senderId) => {
      LinkuraMod.Logger.Info($"[LinkuraSkinSync] Received msg from peer {senderId}: {msg}.");
      LinkuraNetwork.SetState(senderId, msg);
    });
  }

  [HarmonyPatch(typeof(NetClientGameService), nameof(NetClientGameService.OnConnectedToHost))]
  [HarmonyPostfix]
  public static void ClientConnected_Postfix(NetClientGameService __instance) {
    __instance.SendMessage(LinkuraNetworkState.Create());
  }

  [HarmonyPatch(typeof(NetHostGameService), nameof(NetHostGameService.OnPeerConnected))]
  [HarmonyPostfix]
  public static void HostPeerConnected_Postfix(NetHostGameService __instance, ulong peerId) {
    __instance.SendMessage(LinkuraNetworkState.Create(), peerId);
  }

  [HarmonyPatch(typeof(NMerchantRoom), "AfterRoomIsLoaded")]
  [HarmonyPostfix]
  public static void AfterRoomIsLoaded_Postfix(NMerchantRoom __instance) {
    List<Player> players = Traverse.Create(__instance).Field<List<Player>>("_players").Value;
    IReadOnlyList<NMerchantCharacter> visuals = __instance.PlayerVisuals;
    for (int i = 0; i < visuals.Count && i < players.Count; i++) {
      if (visuals[i] is NLinkuraMerchantCharacter character) {
        LinkuraNetwork.ApplySyncedSkin(character.GetNode<Node2D>("SpineSprite"), players[i].NetId);
        // SetSkeletonDataRes resets the spine animation state, so we must re-apply
        // the relaxed animation after the swap. SpineAnimationPatch will remap the
        // vanilla name to the correct Kaho animation for the new skeleton.
        character.PlayAnimation(LinkuraAnimation.VANILLA_ANIM_RELAXED_LOOP, loop: true);
      }
    }
  }
}
