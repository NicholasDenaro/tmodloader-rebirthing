using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Rebirthing
{
  public class RebirthTile : GlobalTile
  {
    public static Dictionary<Point16, List<Point16>> BrokenTilesKeyTL = new Dictionary<Point16, List<Point16>>();
    public static Dictionary<Point16, Point16> BrokenTilesKeyXY = new Dictionary<Point16, Point16>();

    public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
      var tl = TileObjectData.TopLeft(i, j);

      UpdateBrokenTile(i, j, tl.X, tl.Y);
    }

    private static void UpdateBrokenTile(int x, int y, int tlx, int tly)
    {
      Point16 key = new Point16(tlx, tly);
      if (!BrokenTilesKeyTL.ContainsKey(key))
      {
        BrokenTilesKeyTL.Add(key, new List<Point16>());
      }

      Point16 value = new Point16(x, y);
      BrokenTilesKeyTL[key].Add(value);

      // Reverse lookup
      BrokenTilesKeyXY.TryAdd(value, key);
    }

    public static Point16 GetTLForBreakXY(int x, int y)
    {
      Point16 key = new Point16(x, y);
      return BrokenTilesKeyXY[key];
    }

    public static void RemoveForBreakXY(int x, int y)
    {
      var tl = GetTLForBreakXY(x, y);

      List<Point16> xys = BrokenTilesKeyTL[tl];
      BrokenTilesKeyTL.Remove(tl);

      foreach (Point16 xy in xys)
      {
        BrokenTilesKeyXY.Remove(xy);
      }
    }
  }
}