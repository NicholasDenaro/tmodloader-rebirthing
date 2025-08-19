using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
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

      Point16 tl;

      try
      {
        tl = GetTopLeft(i, j);
      }
      catch
      {
        tl = new Point16(i, j);
      }

      UpdateBrokenTile(i, j, tl.X, tl.Y);
    }

    public static Point16 GetTopLeft(int x, int y)
    {
      Tile tile = Main.tile[x, y];

      var tileData = TileObjectData.GetTileData(tile);
      if (tileData == null || tileData.CoordinateFullWidth == 0 || tileData.CoordinateFullHeight == 0)
      {
        return new Point16(x, y);
      }

      try
      {
        return TileObjectData.TopLeft(x, y);
      }
      catch
      {
        return new Point16(x, y);
      }
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
      if (!BrokenTilesKeyXY.ContainsKey(key))
      {
        return Point16.NegativeOne;
      }
      return BrokenTilesKeyXY[key];
    }

    public static void RemoveForBreakXY(int x, int y)
    {
      var tl = GetTLForBreakXY(x, y);

      if (tl == Point16.NegativeOne || !BrokenTilesKeyTL.ContainsKey(tl))
      {
        BrokenTilesKeyXY.Remove(new Point16(x, y));
        return;
      }

      List<Point16> xys = BrokenTilesKeyTL[tl];
      BrokenTilesKeyTL.Remove(tl);

      foreach (Point16 xy in xys)
      {
        BrokenTilesKeyXY.Remove(xy);
      }
    }
  }
}