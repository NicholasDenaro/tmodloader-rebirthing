using System.Linq;
using System.Numerics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Rebirthing.Items
{
  public class DeerclopsSummon : ModItem
  {
    public override void SetStaticDefaults()
    {
      Item.ResearchUnlockCount = 1;
      ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
      NPCID.Sets.MPAllowedEnemies[NPCID.Deerclops] = true;
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
      return !Main.npc.Any(npc => npc.type == NPC.deerclopsBoss);
    }

    public override bool? UseItem(Player player)
    {
      NPC.SpawnBoss((int)player.position.X - 100, (int)player.position.Y, NPCID.Deerclops, player.whoAmI);
      return true;
    }

    public override bool ConsumeItem(Player player)
    {
      return true;
    }
  }
}