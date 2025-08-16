using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Rebirthing.Items
{
  public class SolarSummon : ModItem
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
      return !NPC.TowerActiveSolar;
    }

    public override bool? UseItem(Player player)
    {
      NPC.NewNPC(player.GetSource_ItemUse(Item), (int)player.position.X, (int)player.position.Y - 100, NPCID.LunarTowerSolar, 0);

      NPC.ShieldStrengthTowerSolar = NPC.LunarShieldPowerNormal;

      return true;
    }

    public override bool ConsumeItem(Player player)
    {
      return true;
    }
  }
}