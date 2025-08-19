using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Rebirthing
{
  public class PlayerDetour : ModSystem
  {
    public override void Load()
    {
      Terraria.On_Player.PickTile += this.HandleBlockBreak;
    }

    private void HandleBlockBreak(Terraria.On_Player.orig_PickTile orig, Terraria.Player self, int x, int y, int pickPower)
    {
      if (Rebirthing.IsClient || Rebirthing.IsSinglePlayer)
      {
        if (Rebirthing.Player == self.GetModPlayer<RebirthPlayer>())
        {
          Point16 tl;

          try
          {
            tl = RebirthTile.GetTopLeft(x, y);
          }
          catch
          {
            tl = new Point16(x, y);
          }

          Rebirthing.Player.AddHitToTile(tl.X, tl.Y);
        }
      }

      // Rebirthing.Write("calling original On_Player.PickTile");
      orig(self, x, y, pickPower);
    }
  }
}