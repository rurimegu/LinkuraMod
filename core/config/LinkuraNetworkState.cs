using System.Collections.Generic;
using System.Text;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;
using RuriMegu.Core.Characters.Kaho;
using RuriMegu.Core.Utils;

namespace RuriMegu.Core.Config;

/// <summary>
/// A single packet sent to the party when a player sets or updates their configured skin.
/// </summary>
public struct LinkuraNetworkState : INetMessage, IPacketSerializable {
  public LinkuraNetworkState() { }

  public class CharacterConfig : IPacketSerializable {
    public string SkinName;

    public void Serialize(PacketWriter writer) {
      writer.WriteString(SkinName);
    }

    public void Deserialize(PacketReader reader) {
      SkinName = reader.ReadString();
    }
  }

  //#region Data
  /// <summary>
  /// The net peer ID of the player this state belongs to.
  /// 0 means "self" — the receiver should substitute their own peer ID.
  /// </summary>
  public ulong SenderId { get; set; } = 0;
  public Dictionary<string, CharacterConfig> Characters { get; private set; } = [];
  public string SelectedCharacter { get; set; } = "";
  //#endregion

  public readonly bool ShouldBroadcast => true;

  public readonly NetTransferMode Mode => NetTransferMode.Reliable;

  public readonly LogLevel LogLevel => LogLevel.Info;

  public readonly void Serialize(PacketWriter writer) {
    writer.WriteULong(SenderId);
    writer.Write(Characters);
    writer.WriteString(SelectedCharacter);
  }

  public void Deserialize(PacketReader reader) {
    SenderId = reader.ReadULong();
    reader.Read(Characters);
    SelectedCharacter = reader.ReadString();
  }

  public override string ToString() {
    var sb = new StringBuilder();
    sb.AppendLine($"<LinkuraNetworkState> SelectedCharacter: {SelectedCharacter}");
    foreach (var (characterId, config) in Characters) {
      sb.Append(characterId == SelectedCharacter ? "* " : "  ");
      sb.AppendLine($"{characterId}: {config.SkinName}");
    }
    return sb.ToString();
  }

  public string CurrentSkinName {
    get {
      if (Characters.TryGetValue(SelectedCharacter, out var config)) {
        return config.SkinName;
      }
      LinkuraMod.Logger.Error($"Could not find skin: {this}");
      return SpineSkinLoader.BUILTIN_SKIN_LABEL;
    }
  }
  public static LinkuraNetworkState Create() {
    return new LinkuraNetworkState {
      Characters = new Dictionary<string, CharacterConfig> {
        { HinoshitaKaho.CHARACTER_ID, new CharacterConfig { SkinName = LinkuraModConfig.KahoSkin } }
      },
      SelectedCharacter = HinoshitaKaho.CHARACTER_ID
    };
  }
}

public static class PacketExtensions {
  public static void Write<T>(this PacketWriter writer, Dictionary<string, T> dict) where T : IPacketSerializable {
    writer.WriteInt(dict.Count);
    foreach (var (characterId, config) in dict) {
      writer.WriteString(characterId);
      config.Serialize(writer);
    }
  }

  public static void Read<T>(this PacketReader reader, Dictionary<string, T> dict) where T : IPacketSerializable, new() {
    int count = reader.ReadInt();
    dict.Clear();
    for (int i = 0; i < count; i++) {
      var key = reader.ReadString();
      var value = new T();
      value.Deserialize(reader);
      dict[key] = value;
    }
  }
}
