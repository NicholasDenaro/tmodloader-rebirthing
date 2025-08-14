using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Rebirthing
{
  public class ExpBar : UIState
  {
    private ExpBarImage Image;
    public override void OnInitialize()
    {
      Append(Image = new ExpBarImage());
    }
  }

  public class ExpBarImage : UIElement
  {
    private Asset<Texture2D> texture;
    public ExpBarImage()
    {
      texture = ModContent.Request<Texture2D>("Rebirthing/Assets/Textures/UI/expBar");
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      if (Rebirthing.Player == null || !texture.IsLoaded)
      {
        return;
      }

      this.Width.Pixels = (int)(Main.PendingResolutionWidth / Main.UIScale / 2);
      int width = (int)this.Width.Pixels;

      this.Left.Pixels = (int)(Main.PendingResolutionWidth / Main.UIScale * 0.25f);
      this.Top.Pixels = (Main.PendingResolutionHeight - 24) / Main.UIScale;

      PlayerData data = Rebirthing.Player.RebirthData;
      float percentExp = data.Exp * 1.0f / PlayerData.ExpPerLevel(data.Level);

      float x = this.Left.Pixels;
      float y = this.Top.Pixels;
      float scale = 0.4f;
      int step = (int) (8 * scale);
      this.Height.Pixels = texture.Height() * scale;

      spriteBatch.DrawString(FontAssets.MouseText.Value, "Level: " + data.Level, new Vector2((int)x, (int)y - FontAssets.MouseText.Value.DefaultCharacterData.Glyph.Height * Main.UIScale * 2), Color.White);
      spriteBatch.Draw(this.texture.Value, new Rectangle((int)x, (int)y, 4, 12), new Rectangle(0, 0, 8, 24), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
      spriteBatch.Draw(this.texture.Value, new Rectangle((int)x + 4, (int)y, width, 12), new Rectangle(10, 0, 8, 24), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
      spriteBatch.Draw(this.texture.Value, new Rectangle((int)x + 4, (int)y, (int)(width * percentExp), 12), new Rectangle(30, 0, 8, 24), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
      spriteBatch.Draw(this.texture.Value, new Rectangle((int)x + 4 + width, (int)y, 4, 12), new Rectangle(20, 0, 8, 24), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

      if (IsMouseHovering)
      {
        UICommon.TooltipMouseText(data.Exp + "/" + PlayerData.ExpPerLevel(data.Level) + " Exp");
      }
    }
  }

  [Autoload(Side = ModSide.Client)]
  public class ExpBarSystem : ModSystem
  {
    internal ExpBar ExpBar;
    private UserInterface _expBar;

    public override void Load()
    {
      ExpBar = new ExpBar();
      ExpBar.Activate();
      _expBar = new UserInterface();
      _expBar.SetState(ExpBar);
    }

    public override void UpdateUI(GameTime gameTime)
    {
      _expBar?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
      int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
      if (mouseTextIndex != -1)
      {
        layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
            "Rebirthing: Exp bar",
            delegate
            {
              _expBar.Draw(Main.spriteBatch, new GameTime());
              return true;
            },
            InterfaceScaleType.UI)
        );
      }
    }
  }
}