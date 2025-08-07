using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace Rebirthing
{
  // Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
  public class Rebirthing : Mod
  {
    public static Rebirthing Instance { get; private set; }
    public static bool IsServer => Main.dedServ;
    public static bool IsClient => !IsServer;
    public static bool IsSinglePlayer => Main.netMode == NetmodeID.SinglePlayer;
    public static bool IsHost => IsServer || IsSinglePlayer;

    public static float ExpRate;
    public static float CoinRate;
    public static float SpecsRate;
    public static float DropRate;
    public static float DropCountRate;

    public static RebirthPlayer Player;

    public static List<RebirthPlayer> Players { get; } = new List<RebirthPlayer>();

    public override void Load()
    {
      Instance = this;
      if (IsClient)
      {
        sendConnectMessage();
      }
      Console.WriteLine("Loaded Rebirthing");
    }

    private void sendConnectMessage()
    {
      if (!IsSinglePlayer)
      {
        ModPacket packet = GetPacket();
        packet.Write((byte)MessageType.CONNECT);
        packet.Send();
      }
    }

    private void sendNpcKilledMessage(int whoAmI)
    {
      ModPacket packet = GetPacket();
      packet.Write((byte)MessageType.NPC_KILLED);
      packet.Write(whoAmI);
      packet.Send();
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
      if (IsServer)
      {
        HandlePacketOnServer(reader, whoAmI);
      }
      else if (IsClient)
      {
        HandlePacketOnClient(reader, whoAmI);
      }
    }

    private void HandlePacketOnClient(BinaryReader reader, int whoAmI)
    {
      MessageType type = (MessageType)reader.ReadByte();

      switch (type)
      {
        case MessageType.NPC_KILLED:
          int npc = reader.ReadInt32();
          AwardExp(npc);
          break;
        case MessageType.MINING:
          int x = reader.ReadInt32();
          int y = reader.ReadInt32();
          int exp = reader.ReadInt32();
          this.AwardExpForMining(x, y, exp);
          break;
      }
    }

    private void HandlePacketOnServer(BinaryReader reader, int whoAmI)
    {
      MessageType type = (MessageType)reader.ReadByte();

      switch (type)
      {
        case MessageType.CONNECT:
          break;
      }
    }

    public void AwardExp(int whoAmI)
    {
      if (IsClient)
      {
        foreach (RebirthPlayer player in Players)
        {
          player.killNpc(whoAmI);
        }
      }
      else if (IsServer)
      {
        sendNpcKilledMessage(whoAmI);
      }
    }

    public static void Write(string message)
    {
      Main.NewText(message);
      Console.WriteLine(message);
    }

    internal void AwardExpForMining(int x, int y, int exp)
    {
      if (Rebirthing.IsServer)
      {
        ModPacket packet = GetPacket();
        packet.Write((byte)MessageType.MINING);
        packet.Write(x);
        packet.Write(y);
        packet.Write(exp);
        packet.Send();
      }
      else
      {
        foreach (RebirthPlayer player in Players)
        {
          player.AwardExpForMining(x, y, exp);
        }
      }
    }
  }

  public class ExpCommand : ModCommand
  {
    public override string Command => "exp";

    public override CommandType Type => CommandType.World;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
      Rebirthing.Player.AwardExp(int.Parse(args[0]));
    }
  }

  public enum MessageType { CONNECT = 0, NPC_KILLED, MINING }
}
