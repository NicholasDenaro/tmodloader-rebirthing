using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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

    public static RebirthPlayer Player {
      get
      {
        
        try
        {
          if (Main.myPlayer >= 0 && Main.myPlayer < Main.player.Length && Main.myPlayer != 255)
          {
            return Main.LocalPlayer.GetModPlayer<RebirthPlayer>();
          }
        }
        catch
        {
        }
        
        return null;
      }
    }

    public List<RebirthPlayer> Players { get; } = new List<RebirthPlayer>();

    public static Dictionary<string, bool> BossKilled = new Dictionary<string, bool>();

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
          int npcWhoAmI = reader.ReadInt32();
          AwardKillExp(npcWhoAmI);
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
        case MessageType.SYNC_STATS:
          int playerWhoAmI = reader.ReadInt32();
          RebirthPlayer player = Main.player[playerWhoAmI].GetModPlayer<RebirthPlayer>();
          if (!this.Players.Contains(player))
          {
            this.Players.Add(player);
          }
          int attributesCount = reader.ReadInt32();
          for (int i = 0; i < attributesCount; i++)
          {
            string name = reader.ReadString();
            int level = reader.ReadInt32();
            RebirthAttribute attr = new RebirthAttribute()
            {
              Id = name,
              Level = level
            };
            player.SetAttribute(attr);
          }
          int attributesTCount = reader.ReadInt32();
          for (int i = 0; i < attributesTCount; i++)
          {
            string name = reader.ReadString();
            int level = reader.ReadInt32();
            RebirthAttribute attr = new RebirthAttribute()
            {
              Id = name,
              Level = level
            };
            player.SetTAttribute(attr);
          }
          break;
        case MessageType.INCREMENT_WORLD:
          this.IncrementWorld(false);
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
        case MessageType.SYNC_STATS:
          int playerWhoAmI = reader.ReadInt32();
          RebirthPlayer player = Main.player[playerWhoAmI].GetModPlayer<RebirthPlayer>();
          if (!this.Players.Contains(player))
          {
            this.Players.Add(player);
          }
          int attributesCount = reader.ReadInt32();
          for (int i = 0; i < attributesCount; i++)
          {
            string name = reader.ReadString();
            int level = reader.ReadInt32();
            RebirthAttribute attr = new RebirthAttribute()
            {
              Id = name,
              Level = level
            };
            player.SetAttribute(attr);
          }
          int attributesTCount = reader.ReadInt32();
          for (int i = 0; i < attributesTCount; i++)
          {
            string name = reader.ReadString();
            int level = reader.ReadInt32();
            RebirthAttribute attr = new RebirthAttribute()
            {
              Id = name,
              Level = level
            };
            player.SetTAttribute(attr);
          }
          player.SyncWithServer();
          break;
        case MessageType.INCREMENT_WORLD:
          this.IncrementWorld(false);
          break;
      }
    }

    public void AwardKillExp(int npcWhoAmI)
    {
      if (IsClient)
      {
        foreach (RebirthPlayer player in Players)
        {
          player.killNpc(npcWhoAmI);
        }
      }
      else if (IsServer)
      {
        sendNpcKilledMessage(npcWhoAmI);
      }
    }

    public static void Write(string message, byte r = 255, byte g = 255, byte b = 255)
    {
      Console.WriteLine(message);
      Main.NewText(message, r, g, b);

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
        Rebirthing.Player.AwardExpForMining(x, y, exp);
      }
    }

    internal void AwardExpForCrafting(int exp)
    {
      Player.AwardExp(exp);
    }

    public void Reset()
    {
      Main.GameMode = GameModeID.Normal;
      this.WorldIncrement = 0;
      Main.hardMode = false;
    }

    public void IncrementWorld(bool sendMessage)
    {
      if (!IsHost && sendMessage)
      {
        ModPacket packet = GetPacket();
        packet.Write((byte)MessageType.INCREMENT_WORLD);
        packet.Send();
        return;
      }

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

      Main.hardMode = false;

      if (Main.GameModeInfo.Id == GameModeID.Normal)
      {
        Main.GameMode = GameModeID.Expert;
        WorldIncrement = 0;
      }
      else if (Main.GameModeInfo.Id == GameModeID.Expert)
      {
        Main.GameMode = GameModeID.Master;
        WorldIncrement = 0;
      }
      else
      {
        WorldIncrement++;
        SetDifficulty(Main.GameMode, WorldIncrement);
      }

      if (IsHost)
      {
        Rebirthing.Write("The world shifts and grows stronger", 50, 50, 50);
      }

      if (IsServer)
      {
        ModPacket packet = GetPacket();
        packet.Write((byte)MessageType.INCREMENT_WORLD);
        packet.Send();
      }
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
      if (Rebirthing.IsSinglePlayer)
      {
        return;
      }

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

  public class ShowIncrementWorldCommand : ModCommand
  {
    public override string Command => "world";

    public override CommandType Type => CommandType.World;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
      Rebirthing.Write("World game mode: " + Main.GameModeInfo);
      Rebirthing.Write("World Increment: " + Rebirthing.Instance.WorldIncrement);
    }
  }

  public class ResetCommand : ModCommand
  {
    public override string Command => "reset";

    public override CommandType Type => CommandType.World;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
      Rebirthing.Instance.Reset();
    }
  }

  public class IncrementCommand : ModCommand
  {
    public override string Command => "increment";

    public override CommandType Type => CommandType.World;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
      Rebirthing.Instance.IncrementWorld(true);
    }
  }

  public class RebirthingSystem : ModSystem
  {
    public override void SaveWorldData(TagCompound tag)
    {
      tag.Set("WorldIncrement", Rebirthing.Instance.WorldIncrement);
      tag.Set("BossKilled", JsonSerializer.Serialize(Rebirthing.BossKilled));
    }

    public override void LoadWorldData(TagCompound tag)
    {
      if (tag.ContainsKey("WorldIncrement"))
      {
        Rebirthing.Instance.WorldIncrement = tag.Get<int>("WorldIncrement");

        if (Rebirthing.Instance.WorldIncrement > 0)
        {
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
      }
      else
      {
        Rebirthing.Instance.WorldIncrement = 0;
      }

      if (tag.ContainsKey("BossKilled"))
      {
        Rebirthing.BossKilled = JsonSerializer.Deserialize<Dictionary<string, bool>>(tag.Get<string>("BossKilled"));
      }
      else
      {
        Rebirthing.BossKilled = new Dictionary<string, bool>();
      }
    }
  }

  public enum MessageType { CONNECT = 0, NPC_KILLED, MINING, MESSAGE, DIFFICULTY, GET_DIFFICULTY, SYNC_STATS, INCREMENT_WORLD, SPAWN_BOSS }
}
