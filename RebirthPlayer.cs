using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Rebirthing
{
  public class RebirthPlayer : ModPlayer
  {
    public PlayerData RebirthData { get; private set; }

    private Dictionary<int, int> damageToNpc = new Dictionary<int, int>();

    private List<int> killedNpcs = new List<int>();

    public Projectile Ring { get; private set; }
    public Projectile Flash { get; private set; }
    private bool rebirthing = false;
    public bool IsRebirthing => this.rebirthing;
    private int rebirthTimer = -1;
    public int RebirthTimer => this.rebirthTimer;

    private int levelUps = 0;
    private int levelUpsTimer = 0;

    public List<int> levelUpsTimers = new List<int>();

    class BrokenTile
    {
      public int X { get; set; }
      public int Y { get; set; }

      public BrokenTile(int x, int y)
      {
        this.X = x;
        this.Y = y;
      }
    }

    private List<BrokenTile> brokenTiles = new List<BrokenTile>();

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
      int damageSpec = this.GetAttribute("Damage").Level;

      double damage = this.Player.GetTotalDamage(modifiers.DamageType).ApplyTo(Player.HeldItem.damage + damageSpec);

      modifiers.SourceDamage.Base += damageSpec;

      modifiers.CritDamage += this.GetAttribute("Crit Damage").Level / 20.0f;

      if (!damageToNpc.ContainsKey(target.whoAmI))
      {
        damageToNpc.Add(target.whoAmI, 0);
      }

      damageToNpc[target.whoAmI] += Math.Min((int)damage, target.life);
    }

    public override void ModifyCaughtFish(Item fish)
    {
      this.AwardExp(fish.value / 10);
    }

    public override void PreUpdate()
    {

      if (PlayerInput.Triggers.JustPressed.Inventory && ModContent.GetInstance<RebirthingSpecsSystem>().IsOpen)
      {
        ModContent.GetInstance<RebirthingSpecsSystem>().Hide();
      }

      foreach (int whoAmI in killedNpcs)
        {
          if (this.damageToNpc.ContainsKey(whoAmI))
          {
            int damage = this.damageToNpc[whoAmI];

            this.AwardExp(damage);

            this.damageToNpc.Remove(whoAmI);
          }
        }

      killedNpcs.Clear();

      foreach (HitTile.HitTileObject data in this.Player.hitTile?.data)
      {
        if (data.type != 0)
        {
          brokenTiles.Add(new BrokenTile(data.X, data.Y));
        }
      }

      if (this.rebirthing)
      {
        this.Player.moveSpeed = 0;
        this.Player.gravity = 0;
        this.Player.velocity = Vector2.Zero;

      }

      if (this.rebirthTimer > 4 * 60)
      {
        this.Player.velocity.Y = -0.6f;
      }
      else if (this.rebirthTimer == 4 * 60)
      {
        // TODO: Make player invuln, prevent moving, and then play effect
        // Glow_109 // fire ring? // GlowMaskID.CultistTabletBack;
        // Extra_34 // inner hex?
        // Projectile_673 // purple hexy thing?
        // Projectile_490 // Large outer ring

        int id = Projectile.NewProjectile(this.Player.GetSource_FromThis(), this.Player.Center, Vector2.Zero, ProjectileID.CultistRitual, 0, 0, this.Player.whoAmI);

        Projectile proj = Main.projectile[id];

        proj.scale = 0;
        proj.timeLeft = 5 * 60;
        proj.aiStyle = 0;
        proj.friendly = true;
        proj.light = 0f;
        proj.tileCollide = false;

        this.Ring = proj;
      }
      else if (this.rebirthTimer >= 3 * 60)
      {
        this.Ring.scale += 1.0f / 60;
        this.Ring.rotation += (float)Math.PI / 15;
        this.Ring.light += 1.0f / 60;
      }
      else if (this.rebirthTimer > 2 * 60)
      {
      }
      else if (this.rebirthTimer == 2 * 60)
      {
      }
      else if (this.rebirthTimer >= 1 * 60)
      {

      }
      else if (this.rebirthTimer > 45)
      {
        this.Ring.scale -= 1.0f / 15;
        this.Ring.rotation += (float)Math.PI / 15;
        this.Ring.light -= 1.0f / 15;
      }
      else if (this.rebirthTimer == 45)
      {
        if (this.Ring?.active == true)
        {
          this.Ring.Kill();
        }

        // Using Projectile 79
        // Maybe use Projectile 540 instead
        int id = Projectile.NewProjectile(this.Player.GetSource_FromThis(), this.Player.Center, Vector2.Zero, ProjectileID.RainbowRodBullet, 0, 0, this.Player.whoAmI);

        Projectile proj = Main.projectile[id];

        proj.scale = 0.1f;
        proj.timeLeft = 5 * 60;
        proj.aiStyle = 0;
        proj.friendly = true;
        proj.light = 0f;
        proj.tileCollide = false;
        proj.maxPenetrate = 10000;
        proj.numHits = 10000;
        proj.penetrate = 10000;

        this.Flash = proj;
      }
      else if (this.rebirthTimer > 0)
      {
        this.Flash.scale *= 1.3f;
      }
      else if (this.rebirthTimer == 0)
      {
        if (this.Flash?.active == true)
        {
          this.Flash.Kill();
        }
        this.Flash = null;

        this.Ring = null;
        this.EndRebirth();
      }

      if (this.rebirthTimer >= 0)
      {
        this.rebirthTimer--;
      }

      if (this.levelUps > 0)
      {
        if (this.levelUpsTimer == 0)
        {
          this.levelUpsTimer = 20;

          // Projectile_536 // ProjectileID.MedusaHeadRay // didn't work
          // ProjectileID.StarWrath // maybe
          // ProjectileID.FairyQueenSunDance // maybe
          // ProjectileID.FairyQueenMagicItemShot // didn't work
          // ProjectileID.VoidLens // didn't work

          levelUpsTimers.Add(30);

          // this.Player.chatOverhead = new Player.OverheadMessage();
          // this.Player.chatOverhead.chatText = "Level Up";
          // this.Player.chatOverhead.timeLeft = 30;
          // this.Player.chatOverhead.color = Color.White;

          AdvancedPopupRequest info = new AdvancedPopupRequest();
          info.Text = "Level Up";
          // info.Velocity = new Vector2(0, -1);
          info.Color = Color.White;
          info.DurationInFrames = 60;
          int text = PopupText.NewText(info, this.Player.Top);

          this.levelUps--;
        }

        this.levelUpsTimer--;
      }
    }

    public override void OnEnterWorld()
    {
      Console.WriteLine("Enter World Loading player " + this.Player?.name);
      Rebirthing.Players.Add(this);
      Rebirthing.Player = this;
      if (this.RebirthData == null)
      {
        this.RebirthData = new PlayerData();
      }
      Main.blockInput = false;
    }

    public override void PostUpdateMiscEffects()
    {
      this.Player.moveSpeed += this.GetAttribute("Speed").Level * 0.1f;

      this.Player.GetAttackSpeed(DamageClass.Default) = this.GetAttribute("Attack Speed").Level * 0.1f;

      this.Player.statDefense += this.GetAttribute("Defense").Level * 2;

      this.Player.lifeRegen += this.GetAttribute("Health Regen").Level;

      this.Player.manaRegen += this.GetAttribute("Mana Regen").Level;

      this.Player.GetArmorPenetration(DamageClass.Default) += this.GetAttribute("Armor Pen").Level;

      this.Player.GetCritChance(DamageClass.Default) += this.GetAttribute("Crit Rate").Level * 5;

      this.Player.maxMinions += this.GetAttribute("Max Minions").Level;

      this.Player.pickSpeed -= this.GetAttribute("Block Break Speed").Level * 0.1f;
      this.Player.tileSpeed -= this.GetAttribute("Block Break Speed").Level * 0.1f;

      this.Player.blockRange = (int)(this.Player.blockRange * (1 + this.GetAttribute("Reach").Level * 0.1f));
    }

    public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
    {
      health = StatModifier.Default;
      mana = StatModifier.Default;
      if (this.RebirthData != null)
      {
        health.Base = this.GetAttribute("Health").Level * 10;
        mana.Base = this.GetAttribute("Health").Level * 10;
      }
    }

    public override void Load()
    {
      if (Rebirthing.IsServer)
      {
        Console.WriteLine("Server Loading player " + this.Player?.name);
        Rebirthing.Players.Add(this);
      }
    }

    public override void Unload()
    {
      Console.WriteLine("Unloading player " + this.Player?.name);
      Rebirthing.Players.Remove(this);
    }

    public override void SaveData(TagCompound tag)
    {
      tag.Set("rebirthing", JsonSerializer.Serialize(this.RebirthData), true);
      Console.WriteLine("LOG: Saved player data");
    }

    public override void LoadData(TagCompound tag)
    {
      this.RebirthData = JsonSerializer.Deserialize<PlayerData>(tag.Get<string>("rebirthing")) ?? new PlayerData();
      Console.WriteLine("LOG: Loaded player data");
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
    }

    public void killNpc(int whoAmI)
    {
      killedNpcs.Add(whoAmI);
    }

    public void AwardExpForMining(int x, int y, int exp)
    {
      if (Rebirthing.IsSinglePlayer || Rebirthing.IsClient)
      {
        if (this.brokenTiles.Any(tile => tile.X == x && tile.Y == y))
        {
          this.AwardExp(exp);
        }
      }
    }

    public void AwardExp(int exp)
    {
      exp = (int)(exp * Rebirthing.ExpRate);
      this.levelUps += this.RebirthData.AddExP(exp);
    }

    public void Rebirth()
    {
      this.RebirthData.Rebirth();

      this.rebirthTimer = 5 * 60;
      this.rebirthing = true;
      Main.blockInput = true;
    }

    public void EndRebirth()
    {
      this.rebirthing = false;
      Main.blockInput = false;
    }

    public RebirthAttribute GetAttribute(string name)
    {
      if (this.RebirthData.Attributes.ContainsKey(name))
      {
        return this.RebirthData.Attributes[name];
      }
      else
      {
        return new RebirthAttribute()
        {
          Id = name,
          Level = 0
        };
      }
    }
  }

  public class RebirthPlayerDrawRebirthAuraLayer : PlayerDrawLayer
  {
    private int index = 0;
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
      return drawInfo.drawPlayer.GetModPlayer<RebirthPlayer>().RebirthTimer >= 0;
    }

    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
      if (drawInfo.shadow != 0)
      {
        return;
      }

      Asset<Texture2D> texture = TextureAssets.GlowMask[GlowMaskID.CultistTabletBack];

      int width = texture.Width();
      int height = texture.Height() / 4;

      Vector2 position = drawInfo.Center + new Vector2(0, drawInfo.drawPlayer.height + height * 0.9f) - Main.screenPosition;
      position = new Vector2((int)position.X, (int)position.Y);

      drawInfo.DrawDataCache.Add(new DrawData(texture.Value, position, new Rectangle(0, height * ((index++ % 32) / 8), width, height), Color.White, 0, texture.Size() * 0.5f, 1f, SpriteEffects.None, 0));
    }
  }

  public class RebirthPlayerDrawLevelupLayer : PlayerDrawLayer
  {
    private int index = 0;
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
      return drawInfo.drawPlayer.GetModPlayer<RebirthPlayer>().levelUpsTimers.Count > 0;
    }

    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Leggings);

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
      if (drawInfo.shadow != 0)
      {
        return;
      }

      Asset<Texture2D> texture = TextureAssets.Projectile[ProjectileID.VoidLens];

      if (!texture.IsLoaded)
      {
        texture = Main.Assets.Request<Texture2D>(texture.Name, AssetRequestMode.ImmediateLoad);
      }

      int width = texture.Width();
      int height = texture.Height() / 8;

      Vector2 position = drawInfo.drawPlayer.Center + new Vector2(0, drawInfo.drawPlayer.height * 0.6f) - Main.screenPosition;
      position = new Vector2((int)position.X, (int)position.Y);

      List<int> timers = drawInfo.drawPlayer.GetModPlayer<RebirthPlayer>().levelUpsTimers;
      for (int i = timers.Count - 1; i >= 0; i--)
      {
        int time = timers[i]--;

        float scale = Math.Min((30 - time) / 10.0f, 3);

        Vector2 offset = new Vector2();
        offset.X = width / 2;
        offset.Y = height * 0.85f;

        float alpha = (time < 15 ? (time) : (30 - time)) / 15.0f;

        DrawData data = new DrawData(texture.Value, position, new Rectangle(0, height * ((index++ % 32) / 4), width, height), Color.White * alpha, 0, offset, scale, SpriteEffects.None, 0);

        
        drawInfo.DrawDataCache.Add(data);

        if (time <= 0)
        {
          timers.RemoveAt(i);
        }
      }
    }
  }
}