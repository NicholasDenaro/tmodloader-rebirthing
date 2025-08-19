using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
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


    private bool transcending = false;
    public bool IsTranscending => this.transcending;
    private int transcendTimer = -1;
    public int TranscendTimer => this.transcendTimer;

    private int levelUps = 0;
    private int levelUpsTimer = 0;

    public List<int> levelUpsTimers = new List<int>();

    private Item lastHeldItem;
    private int lastHeldItemStartingReach;

    private List<Point16> brokenTiles = new List<Point16>();

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
      float critDamage = this.GetAttributeValue("Crit Damage");
      float critDamageT = 1 + this.GetTAttributeValue("Crit Damage");

      // Takes into account the rate
      modifiers.CritDamage += critDamage * critDamageT;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
      int damage = damageDone;

      if (target.life < 0)
      {
        damage = damage + target.life;
      }

      if (!damageToNpc.ContainsKey(target.whoAmI))
      {
        damageToNpc.Add(target.whoAmI, 0);
      }

      damageToNpc[target.whoAmI] += damage;
    }

    public override void ModifyCaughtFish(Item fish)
    {
      this.AwardExp(fish.value / 25);
    }

    public override void PreUpdate()
    {

      if (PlayerInput.Triggers.JustPressed.Inventory && ModContent.GetInstance<RebirthingSpecsSystem>().IsOpen)
      {
        ModContent.GetInstance<RebirthingSpecsSystem>().Hide();
      }

      int[] killed = killedNpcs.ToArray();
      killedNpcs.Clear();

      foreach (int npcWhoAmI in killed)
      {
        if (this.damageToNpc.ContainsKey(npcWhoAmI))
        {
          int damage = this.damageToNpc[npcWhoAmI];

          this.AwardExp(damage);

          this.damageToNpc.Remove(npcWhoAmI);
        }
      }

      // foreach (HitTile.HitTileObject data in this.Player.hitTile?.data)
      // {
      //   if (data.type != 0 && data.damage > 0)
      //   {
      //     if (Rebirthing.IsClient)
      //     {
      //       Rebirthing.Write("type: " + data.type + " | damage > 0? " + (data.damage > 0));
      //     }
      //     brokenTiles.Add(new BrokenTile(data.X, data.Y));
      //   }
      // }

      RebirthTick();
      TranscendTick();

      if (this.levelUps > 0)
      {
        if (this.levelUpsTimer == 0)
        {
          this.levelUpsTimer = 20;
          levelUpsTimers.Add(30);

          AdvancedPopupRequest info = new AdvancedPopupRequest();
          info.Text = "Level Up";
          info.Color = Color.White;
          info.DurationInFrames = 60;
          int text = PopupText.NewText(info, this.Player.Top);

          this.levelUps--;
        }

        this.levelUpsTimer--;
      }
    }

    public void AddHitToTile(int x, int y)
    {
      if (Rebirthing.IsServer)
      {
        // The client handles their own block breaking
        return;
      }

      brokenTiles.Add(new Point16(x, y));
    }

    private void RebirthTick()
    {
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
    }

    private void TranscendTick()
    {
      if (this.transcending)
      {
        this.Player.moveSpeed = 0;
        this.Player.gravity = 0;
        this.Player.velocity = Vector2.Zero;
      }

      if (this.transcendTimer > 4 * 60)
      {
        this.Player.velocity.Y = -0.6f;
      }
      else if (this.transcendTimer == 4 * 60)
      {
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
      else if (this.transcendTimer >= 3 * 60)
      {
        this.Ring.scale += 1.0f / 60;
        this.Ring.rotation += (float)Math.PI / 15;
        this.Ring.light += 1.0f / 60;
      }
      else if (this.transcendTimer > 2 * 60)
      {
      }
      else if (this.transcendTimer == 2 * 60)
      {
      }
      else if (this.transcendTimer >= 1 * 60)
      {

      }
      else if (this.transcendTimer > 45)
      {
        this.Ring.scale -= 1.0f / 15;
        this.Ring.rotation += (float)Math.PI / 15;
        this.Ring.light -= 1.0f / 15;
      }
      else if (this.transcendTimer == 45)
      {
        if (this.Ring?.active == true)
        {
          this.Ring.Kill();
        }

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
      else if (this.transcendTimer > 0)
      {
        this.Flash.scale *= 1.3f;
      }
      else if (this.transcendTimer == 0)
      {
        if (this.Flash?.active == true)
        {
          this.Flash.Kill();
        }
        this.Flash = null;

        this.Ring = null;
        this.EndTranscend();
      }

      if (this.transcendTimer >= 0)
      {
        this.transcendTimer--;
      }
    }

    public override void OnEnterWorld()
    {
      Console.WriteLine("Enter World Loading player " + this.Player?.name);
      Rebirthing.Instance.Players.Add(this);
      if (this.RebirthData == null)
      {
        this.RebirthData = new PlayerData();
      }
      Main.blockInput = false;

      Rebirthing.Instance.GetDifficulty();

      this.SyncWithServer();
    }

    public override void PostUpdateMiscEffects()
    {
      this.Player.moveSpeed = this.Player.moveSpeed * (1 + this.GetAttributeValue("Speed")) * (1 + this.GetTAttributeValue("Speed"));

      this.Player.GetAttackSpeed(DamageClass.Generic) = (this.Player.GetAttackSpeed(DamageClass.Generic) + this.GetAttributeValue("Attack Speed")) * (1 + this.GetTAttributeValue("Attack Speed"));

      this.Player.statDefense = (this.Player.statDefense + (int)this.GetAttributeValue("Defense")) * (1 + this.GetTAttributeValue("Defense"));

      this.Player.lifeRegen = (int)((this.Player.lifeRegen + this.GetAttributeValue("Health Regen")) * (1 + this.GetTAttributeValue("Health Regen")));

      this.Player.manaRegen = (int)((this.Player.manaRegen + this.GetAttributeValue("Mana Regen")) * (1 + this.GetTAttributeValue("Mana Regen")));

      this.Player.manaCost = Math.Max(0, (this.Player.manaCost - this.GetAttributeValue("Mana Reduction")) * (1 - this.GetTAttributeValue("Mana Reduction")));

      this.Player.GetArmorPenetration(DamageClass.Generic) = (this.Player.GetArmorPenetration(DamageClass.Generic) + this.GetAttributeValue("Armor Pen")) * (1 + this.GetTAttributeValue("Armor Pen"));

      this.Player.GetCritChance(DamageClass.Generic) = (this.Player.GetCritChance(DamageClass.Generic) + this.GetAttributeValue("Crit Rate")) * (1 + this.GetTAttributeValue("Crit Rate"));

      this.Player.GetDamage(DamageClass.Generic).Base += this.GetAttributeValue("Damage");
      this.Player.GetDamage(DamageClass.Generic) += this.GetTAttributeValue("Damage");

      this.Player.maxMinions = (int)((this.Player.maxMinions + this.GetAttributeValue("Max Minions")) * (1 + this.GetTAttributeValue("Max Minions")));

      this.Player.pickSpeed = (this.Player.pickSpeed - this.GetAttributeValue("Block Break Speed")) * (1 - this.GetTAttributeValue("Block Break Speed"));
      this.Player.tileSpeed = (this.Player.tileSpeed - this.GetAttributeValue("Block Break Speed")) * (1 - this.GetTAttributeValue("Block Break Speed"));

      this.UpdateHeldItemReach();

      this.Player.wingTimeMax = (int)((this.Player.wingTimeMax + this.GetAttributeValue("Flight")) * (1 + this.GetTAttributeValue("Flight")));
    }

    private void UpdateHeldItemReach()
    {
      if (this.lastHeldItem != this.Player.HeldItem)
      {
        int startingBoost = this.Player.HeldItem.tileBoost;
        if (this.Player.HeldItem != null)
        {
          this.Player.HeldItem.tileBoost = (int)((this.Player.HeldItem.tileBoost + this.GetAttributeValue("Reach")) * (1 + this.GetTAttributeValue("Reach")));
        }

        if (this.lastHeldItem != null)
        {
          this.lastHeldItem.tileBoost = lastHeldItemStartingReach;
        }

        this.lastHeldItem = this.Player.HeldItem;
        this.lastHeldItemStartingReach = startingBoost;
      }
    }

    private void ResetHeldItemReach()
    {
      if (this.lastHeldItem == this.Player.HeldItem)
      {
        this.Player.HeldItem.tileBoost = this.lastHeldItemStartingReach;
        this.lastHeldItem = null;
        this.lastHeldItemStartingReach = 0;
      }
    }

    public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
    {
      health = StatModifier.Default;
      mana = StatModifier.Default;
      if (this.RebirthData != null)
      {
        health.Base = this.GetAttributeValue("Health");
        health *= 1 + this.GetTAttributeValue("Health");
        mana.Base = this.GetAttributeValue("Mana");
        mana *= 1 + this.GetTAttributeValue("Mana");
      }
    }

    public override void SaveData(TagCompound tag)
    {
      tag.Set("rebirthing", JsonSerializer.Serialize(this.RebirthData), true);
      Console.WriteLine("LOG: Saved player data");
    }

    public override void LoadData(TagCompound tag)
    {
      this.RebirthData = JsonSerializer.Deserialize<PlayerData>(tag.Get<string>("rebirthing")) ?? new PlayerData();

      this.RebirthData.Loadouts = new List<LoadoutData>();
      // if (this.RebirthData.Loadouts.Count == 0)
      {
        for (int i = 0; i < this.Player.Loadouts.Length; i++)
        {
          LoadoutData data = new LoadoutData();
          data.RebirthPoints = this.RebirthData.RebirthPoints;
          foreach (var kvp in this.RebirthData.RebirthAttributes)
          {
            data.RebirthAttributes.Add(kvp.Key, kvp.Value.Clone());
          }
          data.TranscendencePoints = this.RebirthData.TranscendencePoints;
          foreach (var kvp in this.RebirthData.TranscendenceAttributes)
          {
            data.TranscendenceAttributes.Add(kvp.Key, kvp.Value.Clone());
          }

          this.RebirthData.Loadouts.Add(data);
        }
      }
      Console.WriteLine("LOG: Loaded player data");
    }

    public void SyncWithServer()
    {
      if (Rebirthing.IsSinglePlayer)
      {
        return;
      }

      ModPacket packet = Rebirthing.Instance.GetPacket();
      packet.Write((byte)MessageType.SYNC_STATS);
      packet.Write(this.Player.whoAmI);
      packet.Write(this.RebirthData.Loadouts.Count);
      for (int i = 0; i < this.RebirthData.Loadouts.Count; i++)
      {
        packet.Write(this.RebirthData.Loadouts[i].RebirthAttributes.Count);
        foreach (var kvp in this.RebirthData.Loadouts[i].RebirthAttributes)
        {
          packet.Write(kvp.Value.Id);
          packet.Write(kvp.Value.Level);
        }
        packet.Write(this.RebirthData.Loadouts[i].TranscendenceAttributes.Count);
        foreach (var kvp in this.RebirthData.Loadouts[i].TranscendenceAttributes)
        {
          packet.Write(kvp.Value.Id);
          packet.Write(kvp.Value.Level);
        }
      }
      

      if (Rebirthing.IsClient && Main.myPlayer == this.Player.whoAmI)
      {
        packet.Send();
      }
      else if (Rebirthing.IsServer)
      {
        packet.Send(-1, Player.whoAmI);
      }
    }

    public void killNpc(int whoAmI)
    {
      killedNpcs.Add(whoAmI);
    }

    public void AwardExpForMining(int x, int y, int exp)
    {
      if (Rebirthing.IsSinglePlayer || Rebirthing.IsClient)
      {
        int count = this.brokenTiles.RemoveAll(tile => tile.X == x && tile.Y == y);
        RebirthTile.RemoveForBreakXY(x, y);
        if (count > 0)
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
      this.rebirthTimer = 5 * 60;
      this.rebirthing = true;
      Main.blockInput = true;
    }

    public void Transcend()
    {
      this.transcendTimer = 5 * 60;
      this.transcending = true;
      Main.blockInput = true;

      this.SyncWithServer();
    }

    public void EndRebirth()
    {
      if (this.rebirthing)
      {
        this.RebirthData.Rebirth();
      }
      this.rebirthing = false;
      Main.blockInput = false;
    }

    public void EndTranscend()
    {
      if (this.transcending)
      {
        this.RebirthData.Transend();
        this.ResetHeldItemReach();
      }
      this.transcending = false;
      Main.blockInput = false;
    }

    public void Respec()
    {
      this.RebirthData.Respec(this.Player.CurrentLoadoutIndex);
      this.SyncWithServer();
    }

    public void RespecTranscendance()
    {
      this.RebirthData.RespecTranscendance(this.Player.CurrentLoadoutIndex);
      this.SyncWithServer();
    }

    public RebirthAttribute GetAttribute(string name)
    {
      if (this.RebirthData == null)
      {
        this.RebirthData = new PlayerData();
      }
      
      if (this.RebirthData.ActiveLoadout.RebirthAttributes.ContainsKey(name))
      {
        return this.RebirthData.ActiveLoadout.RebirthAttributes[name];
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

    public void SetAttribute(RebirthAttribute attr, int loadoutIndex)
    {
      if (this.RebirthData == null)
      {
        this.RebirthData = new PlayerData();
      }

      if (!this.RebirthData.Loadouts[loadoutIndex].RebirthAttributes.ContainsKey(attr.Id))
      {
        this.RebirthData.Loadouts[loadoutIndex].RebirthAttributes[attr.Id] = attr;
      }
      else
      {
        this.RebirthData.Loadouts[loadoutIndex].RebirthAttributes[attr.Id].Level = attr.Level;
      }

      if (attr.Id == "Reach")
      {
        this.ResetHeldItemReach();
      }

      this.SyncWithServer();
    }

    public RebirthAttribute GetTAttribute(string name)
    {
      if (this.RebirthData == null)
      {
        this.RebirthData = new PlayerData();
      }

      if (this.RebirthData.ActiveLoadout.TranscendenceAttributes.ContainsKey(name))
      {
        return this.RebirthData.ActiveLoadout.TranscendenceAttributes[name];
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

    public void SetTAttribute(RebirthAttribute attr, int loadoutIndex)
    {
      if (this.RebirthData == null)
      {
        this.RebirthData = new PlayerData();
      }

      if (!this.RebirthData.Loadouts[loadoutIndex].TranscendenceAttributes.ContainsKey(attr.Id))
      {
        this.RebirthData.Loadouts[loadoutIndex].TranscendenceAttributes[attr.Id] = attr;
      }
      else
      {
        this.RebirthData.Loadouts[loadoutIndex].TranscendenceAttributes[attr.Id].Level = attr.Level;
      }

      if (attr.Id == "Reach")
      {
        this.ResetHeldItemReach();
      }

      this.SyncWithServer();
    }

    public float GetAttributeValue(string name)
    {
      int level = this.GetAttribute(name).Level;
      float perLevel = float.Parse(Language.GetTextValue($"Mods.Rebirthing.Attributes.{name}.Rebirth.Value"));

      return level * perLevel;
    }

    public float GetTAttributeValue(string name)
    {
      int level = this.GetTAttribute(name).Level;
      float perLevel = float.Parse(Language.GetTextValue($"Mods.Rebirthing.Attributes.{name}.Transcendence.Value"));

      return level * perLevel;
    }

    public int GetAttributeCost(string name, int? lvl = null)
    {
      int level = lvl ?? this.GetAttribute(name).Level;
      float baseCost = float.Parse(Language.GetTextValue($"Mods.Rebirthing.Attributes.{name}.Rebirth.Cost"));
      float scaleRate = float.Parse(Language.GetTextValue($"Mods.Rebirthing.Attributes.{name}.Rebirth.Scale"));
      return (int)(baseCost * Math.Pow(scaleRate, level));
    }

    public int GetTAttributeCost(string name, int? lvl = null)
    {
      int level = lvl ?? this.GetTAttribute(name).Level;
      float baseCost = float.Parse(Language.GetTextValue($"Mods.Rebirthing.Attributes.{name}.Transcendence.Cost"));
      float scaleRate = float.Parse(Language.GetTextValue($"Mods.Rebirthing.Attributes.{name}.Transcendence.Scale"));
      return (int)(baseCost * Math.Pow(scaleRate, level));
    }
  }

  public class RebirthPlayerDrawRebirthAuraLayer : PlayerDrawLayer
  {
    private int index = 0;
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
      return drawInfo.drawPlayer.GetModPlayer<RebirthPlayer>().RebirthTimer >= 0 || drawInfo.drawPlayer.GetModPlayer<RebirthPlayer>().TranscendTimer >= 0;
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