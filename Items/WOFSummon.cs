using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Rebirthing.Items
{
  public class WOFSummon : ModItem
  {
    public override void SetStaticDefaults()
    {
      Item.ResearchUnlockCount = 1;
      ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
      NPCID.Sets.MPAllowedEnemies[NPCID.WallofFlesh] = true;
    }

    public override void SetDefaults()
    {
      Item.consumable = true;
      Item.rare = ItemRarityID.Blue;
      Item.useAnimation = 30;
      Item.useTime = 30;
      Item.useStyle = ItemUseStyleID.HoldUp;
    }

    public override bool CanUseItem(Player player)
    {
      return player.ZoneUnderworldHeight;
    }

    public override bool? UseItem(Player player)
    {
      Vector2 offset = new Vector2();
      if (player.position.X < Main.rightWorld / 2)
      {
        offset.X = -200;
      }
      else
      {
        offset.X = 200;
      }
      NPC.SpawnWOF(player.position + offset);
      return true;
    }

    public override bool ConsumeItem(Player player)
    {
      return true;
    }
  }
}