using System.Collections.Generic;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using RuriMegu.Core.Utils;
using RuriMegu.Nodes.Shop;
using STS2RitsuLib.Patching.Core;
using STS2RitsuLib.Patching.Models;

namespace RuriMegu.Core.Config;

public class LinkuraSkinSyncPatches : IModPatches {
  public static void AddTo(ModPatcher patcher) {
    patcher.RegisterPatch<NetMessageBusConstructorPatch>();
    patcher.RegisterPatch<ClientConnectedPatch>();
    patcher.RegisterPatch<HostPeerConnectedPatch>();
    patcher.RegisterPatch<MerchantRoomAfterLoadedPatch>();
  }
}

public class NetMessageBusConstructorPatch : IPatchMethod {
  public static string PatchId => "skin_sync_net_message_bus_ctor";
  public static string Description => "Register LinkuraNetworkState message handler on NetMessageBus construction";
  public static bool IsCritical => false;

  public static ModPatchTarget[] GetTargets() =>
    [new(typeof(NetMessageBus), ".ctor")];

  public static void Postfix(NetMessageBus __instance) {
    __instance.RegisterMessageHandler<LinkuraNetworkState>((msg, senderId) => {
      ulong actualSender = msg.SenderId != 0 ? msg.SenderId : senderId;
      LinkuraMod.Logger.Info($"[LinkuraSkinSync] Received msg from peer {actualSender} (transport sender {senderId}): {msg}.");
      LinkuraNetwork.SetState(actualSender, msg);
    });
  }
}

public class ClientConnectedPatch : IPatchMethod {
  public static string PatchId => "skin_sync_client_connected";
  public static string Description => "Send own skin state when client connects to host";
  public static bool IsCritical => false;

  public static ModPatchTarget[] GetTargets() =>
    [new(typeof(NetClientGameService), nameof(NetClientGameService.OnConnectedToHost))];

  public static void Postfix(NetClientGameService __instance) {
    __instance.SendMessage(LinkuraNetworkState.Create(__instance.NetId));
  }
}

public class HostPeerConnectedPatch : IPatchMethod {
  public static string PatchId => "skin_sync_host_peer_connected";
  public static string Description => "Send host and other peers' skin states to newly connected peer";
  public static bool IsCritical => false;

  public static ModPatchTarget[] GetTargets() =>
    [new(typeof(NetHostGameService), nameof(NetHostGameService.OnPeerConnected))];

  public static void Postfix(NetHostGameService __instance, ulong peerId) {
    __instance.SendMessage(LinkuraNetworkState.Create(__instance.NetId), peerId);
    foreach (ulong knownPeerId in LinkuraNetwork.GetKnownPeerIds()) {
      if (knownPeerId == peerId) continue;
      LinkuraNetwork.GetStateOrDefault(knownPeerId, out LinkuraNetworkState knownState);
      __instance.SendMessage(knownState, peerId);
    }
  }
}

public class MerchantRoomAfterLoadedPatch : IPatchMethod {
  public static string PatchId => "skin_sync_merchant_room_after_loaded";
  public static string Description => "Apply synced skin to Linkura merchant character after room loads";
  public static bool IsCritical => false;

  public static ModPatchTarget[] GetTargets() =>
    [new(typeof(NMerchantRoom), "AfterRoomIsLoaded")];

  public static void Postfix(NMerchantRoom __instance) {
    List<Player> players = Traverse.Create(__instance).Field<List<Player>>("_players").Value;
    IReadOnlyList<NMerchantCharacter> visuals = __instance.PlayerVisuals;
    for (int i = 0; i < visuals.Count && i < players.Count; i++) {
      if (visuals[i] is NLinkuraMerchantCharacter character) {
        LinkuraNetwork.ApplySyncedSkin(character.GetNode<Node2D>("SpineSprite"), players[i].NetId);
        character.PlayAnimation(LinkuraAnimation.VANILLA_ANIM_RELAXED_LOOP, loop: true);
      }
    }
  }
}
