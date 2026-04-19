using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Config;

public static class LinkuraNetwork {
  public const ulong SINGLE_PLAYER_ID = 1ul;
  private static readonly Dictionary<ulong, LinkuraNetworkState> SyncedStates = [];

  public static void SetState(ulong playerId, LinkuraNetworkState state) {
    SyncedStates[playerId] = state;
    LinkuraMod.Logger.Info($"[LinkuraNetwork] Updated state for peer {playerId}: {state}");
  }

  public static void RemoveState(ulong playerId) {
    if (SyncedStates.Remove(playerId)) {
      LinkuraMod.Logger.Info($"[LinkuraNetwork] Removed state for peer {playerId}.");
    }
  }

  public static void GetStateOrDefault(ulong playerId, out LinkuraNetworkState state) {
    if (!SyncedStates.TryGetValue(playerId, out state)) {
      state = LinkuraNetworkState.Create();
    }
  }

  /// <summary>
  /// A helper method to easily apply the correctly synchronized spine skin for a specific character visual.
  /// </summary>
  public static void ApplySyncedSkin(Node2D spineSpriteElement, ulong playerId) {
    if (spineSpriteElement == null) return;

    GetStateOrDefault(playerId, out LinkuraNetworkState state);
    string skin = state.CurrentSkinName;

    SpineSkinLoader.SwapSkin(skin, new MegaSprite(spineSpriteElement));
  }
}
