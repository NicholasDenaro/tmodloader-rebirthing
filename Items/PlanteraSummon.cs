using System.Numerics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Rebirthing.Items
{
  public class PlanteraSummon : ModItem
  {
    public override void SetStaticDefaults()
    {
      Item.ResearchUnlockCount = 1;
      ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
      NPCID.Sets.MPAllowedEnemies[NPCID.Plantera] = true;
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
      return player.ZoneJungle;
    }

    public override bool? UseItem(Player player)
    {
      NPC.SpawnBoss((int)player.position.X, (int)player.position.Y - 500, NPCID.Plantera, player.whoAmI);
      // NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: NPCID.Plantera);
      return true;
    }

    public override bool ConsumeItem(Player player)
    {
      return true;
    }
  }
}