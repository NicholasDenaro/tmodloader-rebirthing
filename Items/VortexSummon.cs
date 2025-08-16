using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Rebirthing.Items
{
  public class VortexSummon : ModItem
  {
    public override void SetStaticDefaults()
    {
      Item.ResearchUnlockCount = 1;
      ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
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
      return !NPC.TowerActiveVortex;
    }

    public override bool? UseItem(Player player)
    {
      NPC.NewNPC(player.GetSource_ItemUse(Item), (int)player.position.X, (int)player.position.Y - 100, NPCID.LunarTowerVortex, 0);

      NPC.ShieldStrengthTowerVortex = NPC.LunarShieldPowerNormal;

      return true;
    }

    public override bool ConsumeItem(Player player)
    {
      return true;
    }
  }
}