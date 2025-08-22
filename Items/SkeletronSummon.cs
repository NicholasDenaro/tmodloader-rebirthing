using System.Numerics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Rebirthing.Items
{
  public class SkeletronSummon : ModItem
  {
    public override void SetStaticDefaults()
    {
      Item.ResearchUnlockCount = 1;
      ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
      NPCID.Sets.MPAllowedEnemies[NPCID.SkeletronHead] = true;
      NPCID.Sets.MPAllowedEnemies[NPCID.SkeletronHand] = true;
    }

    public override void SetDefaults()
    {
      Item.consumable = true;
      Item.rare = ItemRarityID.Blue;
      Item.useAnimation = 30;
      Item.useTime = 30;
      Item.useStyle = ItemUseStyleID.HoldUp;
      Item.maxStack = 20;
    }

    public override bool CanUseItem(Player player)
    {
      return !Main.dayTime;
    }

    public override bool? UseItem(Player player)
    {
      // NPC.SpawnSkeletron(player.whoAmI); // This kills the clothier

      NPC.SpawnBoss((int)player.position.X, (int)player.position.Y - 200, NPCID.SkeletronHead, player.whoAmI);
      return true;
    }

    public override bool ConsumeItem(Player player)
    {
      return true;
    }
  }
}