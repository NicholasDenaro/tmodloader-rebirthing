using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Rebirthing
{
  public class RebirthItem : GlobalItem
  {
    public override void OnCreated(Item item, ItemCreationContext context)
    {
      Rebirthing.Write("item created " + item.Name);
      Rebirthing.Write("context: " + context);
      if (context is RecipeItemCreationContext)
      {
        RecipeItemCreationContext ricc = (RecipeItemCreationContext)context;

        int exp = (int)(item.value * 1.5 / 5 / 100);

        Rebirthing.Write("Item crafted " + exp + " exp");

        // Rebirthing.Instance.AwardExpForCrafting(exp);
      }
    }

    public override void OnSpawn(Item item, IEntitySource source)
    {
      // Rebirthing.Write("item spawned " + item.Name);
      // Rebirthing.Write("source: " + source);

      if (source is EntitySource_TileBreak)
      {
        EntitySource_TileBreak tbs = (EntitySource_TileBreak)source;
        if (item != null)
        {
          int exp = (int)(item.value * 1.5 / 5 / 100); // buy price. sell price is 1/5, Award exp per silver

          int x = tbs.TileCoords.X;
          int y = tbs.TileCoords.Y;

          Tile tile = Main.tile[x, y];

          if (TileID.Trees == tile.TileType)
          {
            exp = Math.Max(1, exp);
          }

          // Rebirthing.Write("Exp value: " + exp);

          Rebirthing.Instance.AwardExpForMining(x, y, exp);
        }
      }
    }
  }

  public class RebirthItemPotions : GlobalItem
  {

    public override bool InstancePerEntity => true;

    private bool IsFirstUse = true;

    public override bool? UseItem(Item item, Player player)
    {
      if (this.IsFirstUse)
      {
        RebirthPlayer rp = player.GetModPlayer<RebirthPlayer>();
        float rate = 1 + rp.GetAttribute("Buff Duration").Level * 0.1f;
        item.buffTime = (int)(item.buffTime * rate);
      }
      
      this.IsFirstUse = false;

      return null;
    }
  }
}