using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

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

    public int WorldIncrement;

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
        case MessageType.MESSAGE:
          string message = reader.ReadString();
          byte r = reader.ReadByte();
          byte g = reader.ReadByte();
          byte b = reader.ReadByte();
          Rebirthing.Write(message, r, g, b);
          break;
        case MessageType.DIFFICULTY:
          int difficulty = reader.ReadInt32();
          int increment = reader.ReadInt32();
          WorldIncrement = increment;
          this.SetDifficulty(difficulty, increment);
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
        case MessageType.GET_DIFFICULTY:
          ModPacket packet = Rebirthing.Instance.GetPacket();
          packet.Write((byte)MessageType.DIFFICULTY);
          packet.Write(Main.GameMode);
          packet.Write(WorldIncrement);
          packet.Send();
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

    public static void Write(string message, byte r = 255, byte g = 255, byte b = 255)
    {
      Main.NewText(message, r, g, b);
      Console.WriteLine(message);

      if (IsServer)
      {
        ModPacket packet = Rebirthing.Instance.GetPacket();
        packet.Write((byte)MessageType.MESSAGE);
        packet.Write(message);
        packet.Write(r);
        packet.Write(g);
        packet.Write(b);
        packet.Send();
      }
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

    internal void AwardExpForCrafting(int exp)
    {
      Player.AwardExp(exp);
    }

    public void IncrementWorld()
    {
      NPC.downedAncientCultist = false;
      NPC.downedBoss1 = false;
      NPC.downedBoss2 = false;
      NPC.downedBoss3 = false;
      NPC.downedChristmasIceQueen = false;
      NPC.downedChristmasSantank = false;
      NPC.downedChristmasTree = false;
      NPC.downedClown = false;
      NPC.downedDeerclops = false;
      NPC.downedEmpressOfLight = false;
      NPC.downedFishron = false;
      NPC.downedFrost = false;
      NPC.downedGoblins = false;
      NPC.downedGolemBoss = false;
      NPC.downedHalloweenKing = false;
      NPC.downedHalloweenTree = false;
      NPC.downedMartians = false;
      NPC.downedMechBoss1 = false;
      NPC.downedMechBoss2 = false;
      NPC.downedMechBoss3 = false;
      NPC.downedMechBossAny = false;
      NPC.downedMoonlord = false;
      NPC.downedPirates = false;
      NPC.downedPlantBoss = false;
      NPC.downedQueenBee = false;
      NPC.downedQueenSlime = false;
      NPC.downedSlimeKing = false;
      NPC.downedTowerNebula = false;
      NPC.downedTowerSolar = false;
      NPC.downedTowerStardust = false;
      NPC.downedTowerVortex = false;

      if (Main.GameMode == GameModeID.Normal)
      {
        Main.GameMode = GameModeID.Expert;
      }
      else if (Main.GameMode == GameModeID.Expert)
      {
        Main.GameMode = GameModeID.Master;
      }
      else
      {
        WorldIncrement++;
        SetDifficulty(Main.GameMode, WorldIncrement);
      }

      Rebirthing.Write("The world shifts and grows stronger", 50, 50, 50);
    }

    private void SetDifficulty(int gameMode, int increment)
    {
      try
      {
        this.WorldIncrement = increment;
        Main.GameMode = gameMode;
        if (WorldIncrement > 0)
        {
          GameModeData masterData = Main.RegisteredGameModes[GameModeID.Master];
          GameModeData data = Main.GameModeInfo;
          data.EnemyDamageMultiplier = masterData.EnemyDamageMultiplier * (float)Math.Pow(2, Rebirthing.Instance.WorldIncrement);
          data.EnemyDefenseMultiplier = masterData.EnemyDefenseMultiplier * (float)Math.Pow(2, Rebirthing.Instance.WorldIncrement);
          data.EnemyMaxLifeMultiplier = masterData.EnemyMaxLifeMultiplier * (float)Math.Pow(2, Rebirthing.Instance.WorldIncrement);
          data.TownNPCDamageMultiplier = masterData.TownNPCDamageMultiplier * (float)Math.Pow(2, Rebirthing.Instance.WorldIncrement);
          Main.RegisteredGameModes.Add(10 + Rebirthing.Instance.WorldIncrement, data);
          Main.GameMode = 10 + Rebirthing.Instance.WorldIncrement;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    public void GetDifficulty()
    {
      ModPacket packet = GetPacket();
      packet.Write((byte)MessageType.GET_DIFFICULTY);
      packet.Send();
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

  public class IncrementWorldCommand : ModCommand
  {
    public override string Command => "increment";

    public override CommandType Type => CommandType.World;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
      Rebirthing.Instance.IncrementWorld();
    }
  }

  public class ShowIncrementWorldCommand : ModCommand
  {
    public override string Command => "world";

    public override CommandType Type => CommandType.World;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
      Rebirthing.Write("World Increment: " + Rebirthing.Instance.WorldIncrement);
    }
  }

  public class RebirthingSystem : ModSystem
  {
    public override void SaveWorldData(TagCompound tag)
    {
      tag.Set("WorldIncrement", Rebirthing.Instance.WorldIncrement);
    }

    public override void LoadWorldData(TagCompound tag)
    {
      if (tag.ContainsKey("WorldIncrement"))
      {
        Rebirthing.Instance.WorldIncrement = tag.Get<int>("WorldIncrement");

        try
        {
          GameModeData masterData = Main.RegisteredGameModes[GameModeID.Master];
          GameModeData data = Main.GameModeInfo;
          data.EnemyDamageMultiplier = masterData.EnemyDamageMultiplier * (float)Math.Pow(2, Rebirthing.Instance.WorldIncrement);
          data.EnemyDefenseMultiplier = masterData.EnemyDefenseMultiplier * (float)Math.Pow(2, Rebirthing.Instance.WorldIncrement);
          data.EnemyMaxLifeMultiplier = masterData.EnemyMaxLifeMultiplier * (float)Math.Pow(2, Rebirthing.Instance.WorldIncrement);
          data.TownNPCDamageMultiplier = masterData.TownNPCDamageMultiplier * (float)Math.Pow(2, Rebirthing.Instance.WorldIncrement);
          Main.RegisteredGameModes.Add(10 + Rebirthing.Instance.WorldIncrement, data);
          Main.GameMode = 10 + Rebirthing.Instance.WorldIncrement;
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex);
        }
        
      }
      else
      {
        Rebirthing.Instance.WorldIncrement = 0;
      }


    }
  }

  public enum MessageType { CONNECT = 0, NPC_KILLED, MINING, MESSAGE, DIFFICULTY, GET_DIFFICULTY }
}
