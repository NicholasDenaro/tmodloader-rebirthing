using Terraria;
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
      if (Rebirthing.Player == self.GetModPlayer<RebirthPlayer>())
      {
        var tl = TileObjectData.TopLeft(x, y);
        Rebirthing.Player.AddHitToTile(tl.X, tl.Y);
      }

      orig(self, x, y, pickPower);
    }
  }
}