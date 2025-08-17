using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Rebirthing
{
  public class RebirthItem : GlobalItem
  {
    public override void OnCreated(Item item, ItemCreationContext context)
    {
      if (context is RecipeItemCreationContext)
      {
        int exp = (int)(item.value * 1.5 / 5 / 100);

        Rebirthing.Write("Item crafted " + exp + " exp");

        Rebirthing.Instance.AwardExpForCrafting(exp);
      }
    }

    public override void OnSpawn(Item item, IEntitySource source)
    {
      if (Rebirthing.IsServer)
      {
        // Skip since the client will call this and handle exp gains
        return;
      }
      
      if (source is EntitySource_TileBreak tbs)
      {
        if (item != null)
        {
          int exp = (int)(item.value * 1.5 / 5 / 100); // buy price. sell price is 1/5, Award exp per silver

          int x = tbs.TileCoords.X;
          int y = tbs.TileCoords.Y;

          var tl = RebirthTile.GetTLForBreakXY(x, y);

          Rebirthing.Instance.AwardExpForMining(tl.X, tl.Y, exp);
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
        float rate = 1 + rp.GetAttributeValue("Buff Duration");
        float rateX = 1 + rp.GetTAttributeValue("Buff Duration");
        item.buffTime = (int)(item.buffTime * rate * rateX);
      }

      this.IsFirstUse = false;

      return null;
    }
  }
}