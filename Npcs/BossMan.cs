using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Rebirthing.Items;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;

namespace Rebirthing.Npcs
{
  [AutoloadHead]
  public class BossMan : ModNPC
  {
    private static Dictionary<string, bool> bossKilled = new Dictionary<string, bool>();

    private string shopName = "Boss Man Shop";

    public override void SetStaticDefaults()
    {
      Main.npcFrameCount[Type] = 26; // The total amount of frames the NPC has. You may need to change this based on how many frames your sprite sheet has.

      NPCID.Sets.ExtraFramesCount[Type] = 9; // These are the frames for raising their arm, sitting, talking, blinking, and attack. This is the remaining number of frames after the walking frames.
      NPCID.Sets.AttackFrameCount[Type] = 4; // The amount of frames in the attacking animation.
      NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away from the center of the NPC that it tries to attack enemies. There are 16 pixels in 1 tile so a range of 700 is almost 44 tiles.
      NPCID.Sets.AttackType[Type] = 0; // The type of attack the Town NPC performs. 0 = throwing, 1 = shooting, 2 = magic, 3 = melee
      NPCID.Sets.AttackTime[Type] = 90; // The amount of time it takes for the NPC's attack animation to be over once it starts. Measured in ticks. There are 60 ticks per second, so an amount of 90 will take 1.5 seconds.
      NPCID.Sets.AttackAverageChance[Type] = 30; // The denominator for the chance for a Town NPC to attack. Lower numbers make the Town NPC appear more aggressive.
      NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset. Adjust this number to change where on your NPC's head the party hat sits.
    }

    public override void SetDefaults()
    {
      NPC.townNPC = true; // Sets NPC to be a Town NPC
      NPC.friendly = true; // The NPC will not attack player
      NPC.width = 18; // The width of the hitbox (hurtbox)
      NPC.height = 40; // The height of the hitbox (hurtbox)
      NPC.aiStyle = NPCAIStyleID.Passive; // Copies the AI of passive NPCs. This is AI Style 7.
      NPC.damage = 10; // This is the amount of damage the NPC will deal as contact damage. This is NOT the damage dealt by the Town NPC's attack.
      NPC.defense = 15; // All vanilla Town NPCs have a base defense of 15. This will increases as more bosses are defeated.
      NPC.lifeMax = 250; // All vanilla Town NPCs have 250 HP.
      NPC.HitSound = SoundID.NPCHit1; // The sound that is played when the NPC takes damage.
      NPC.DeathSound = SoundID.NPCDeath1; // The sound that is played with the NPC dies.
      NPC.knockBackResist = 0.5f; // All vanilla Town NPCs have 50% knockback resistance. Think of this more as knockback susceptibility. 1f = 100% knockback taken, 0f = 0% knockback taken.

      AnimationType = NPCID.Guide; // Sets the animation style to follow the animation of your chosen vanilla Town NPC.
    }

    public override void LoadData(TagCompound tag)
    {
      BossMan.bossKilled = JsonSerializer.Deserialize<Dictionary<string, bool>>(tag.Get<string>("bosses")) ?? new Dictionary<string, bool>();
    }

    public override void SaveData(TagCompound tag)
    {
      tag.Set("bosses", JsonSerializer.Serialize(BossMan.bossKilled), true);
    }

    public override string GetChat()
    {
      return "I provide items to spawn bosses.";
    }

    public override bool CanTownNPCSpawn(int numTownNPCs)
    {
      this.UpdatingDefeatedBosses();

      return BossMan.bossKilled.Keys.Any(key => BossMan.bossKilled[key]);
    }

    public override bool PreAI()
    {
      this.UpdatingDefeatedBosses();

      return true;
    }

    private void UpdatingDefeatedBosses()
    {
      this.AddOrUpdateBoss("downedAncientCultist", NPC.downedAncientCultist);
      this.AddOrUpdateBoss("downedBoss1", NPC.downedBoss1);
      this.AddOrUpdateBoss("downedBoss2", NPC.downedBoss2);
      this.AddOrUpdateBoss("downedBoss3", NPC.downedBoss3);
      this.AddOrUpdateBoss("downedChristmasIceQueen", NPC.downedChristmasIceQueen);
      this.AddOrUpdateBoss("downedChristmasSantank", NPC.downedChristmasSantank);
      this.AddOrUpdateBoss("downedChristmasTree", NPC.downedChristmasTree);
      this.AddOrUpdateBoss("downedDeerclops", NPC.downedDeerclops);
      this.AddOrUpdateBoss("downedEmpressOfLight", NPC.downedEmpressOfLight);
      this.AddOrUpdateBoss("downedFishron", NPC.downedFishron);
      this.AddOrUpdateBoss("downedFrost", NPC.downedFrost);
      this.AddOrUpdateBoss("downedGoblins", NPC.downedGoblins);
      this.AddOrUpdateBoss("downedGolemBoss", NPC.downedGolemBoss);
      this.AddOrUpdateBoss("downedHalloweenKing", NPC.downedHalloweenKing);
      this.AddOrUpdateBoss("downedMartians", NPC.downedMartians);
      this.AddOrUpdateBoss("downedMechBoss1", NPC.downedMechBoss1);
      this.AddOrUpdateBoss("downedMechBoss2", NPC.downedMechBoss2);
      this.AddOrUpdateBoss("downedMechBoss3", NPC.downedMechBoss3);
      this.AddOrUpdateBoss("downedMoonlord", NPC.downedMoonlord);
      this.AddOrUpdateBoss("downedPirates", NPC.downedPirates);
      this.AddOrUpdateBoss("downedPlantBoss", NPC.downedPlantBoss);
      this.AddOrUpdateBoss("downedQueenBee", NPC.downedQueenBee);
      this.AddOrUpdateBoss("downedQueenSlime", NPC.downedQueenSlime);
      this.AddOrUpdateBoss("downedSlimeKing", NPC.downedSlimeKing);
      this.AddOrUpdateBoss("downedTowerNebula", NPC.downedTowerNebula);
      this.AddOrUpdateBoss("downedTowerSolar", NPC.downedTowerSolar);
      this.AddOrUpdateBoss("downedTowerStardust", NPC.downedTowerStardust);
      this.AddOrUpdateBoss("downedTowerVortex", NPC.downedTowerVortex);
      this.AddOrUpdateBoss("hardMode", Main.hardMode);
    }

    public override void AddShops()
    {
      NPCShop shop = new NPCShop(NPC.type, shopName);

      shop.Add(new NPCShop.Entry(ItemID.SuspiciousLookingEye, new Condition("downedBoss1", () => BossMan.bossKilled.GetValueOrDefault("downedBoss1", false))));

      if (WorldGen.crimson)
      {
        shop.Add(new NPCShop.Entry(ItemID.BloodySpine, new Condition("downedBoss2", () => BossMan.bossKilled.GetValueOrDefault("downedBoss2", false))));
      }
      else
      {
        shop.Add(new NPCShop.Entry(ItemID.WormFood, new Condition("downedBoss2", () => BossMan.bossKilled.GetValueOrDefault("downedBoss2", false))));
      }

      shop.Add<SkeletronSummon>(new Condition("downedBoss3", () => BossMan.bossKilled.GetValueOrDefault("downedBoss3", false)));

      shop.Add(new NPCShop.Entry(ItemID.Abeemination, new Condition("downedQueenBee", () => BossMan.bossKilled.GetValueOrDefault("downedQueenBee", false))));
      shop.Add(new NPCShop.Entry(ItemID.SlimeCrown, new Condition("downedSlimeKing", () => BossMan.bossKilled.GetValueOrDefault("downedSlimeKing", false))));

      shop.Add<WOFSummon>(new Condition("hardMode", () => BossMan.bossKilled.GetValueOrDefault("hardMode", false)));

      shop.Add(new NPCShop.Entry(ItemID.MechanicalWorm, new Condition("downedMechBoss1", () => BossMan.bossKilled.GetValueOrDefault("downedMechBoss1", false))));
      shop.Add(new NPCShop.Entry(ItemID.MechanicalEye, new Condition("downedMechBoss2", () => BossMan.bossKilled.GetValueOrDefault("downedMechBoss2", false))));
      shop.Add(new NPCShop.Entry(ItemID.MechanicalSkull, new Condition("downedMechBoss3", () => BossMan.bossKilled.GetValueOrDefault("downedMechBoss3", false))));

      shop.Add<PlanteraSummon>(new Condition("downedPlantBoss", () => BossMan.bossKilled.GetValueOrDefault("downedPlantBoss", false)));

      shop.Add(new NPCShop.Entry(ItemID.LihzahrdPowerCell, new Condition("downedGolemBoss", () => BossMan.bossKilled.GetValueOrDefault("downedGolemBoss", false))));

      shop.Add<CultistSummon>(new Condition("downedAncientCultist", () => BossMan.bossKilled.GetValueOrDefault("downedAncientCultist", false)));

      shop.Add(new NPCShop.Entry(ItemID.CelestialSigil, new Condition("downedMoonlord", () => BossMan.bossKilled.GetValueOrDefault("downedMoonlord", false))));

      shop.Register();
    }

    private void AddOrUpdateBoss(string name, bool killed)
    {
      if (!BossMan.bossKilled.ContainsKey(name))
      {
        BossMan.bossKilled.Add(name, killed);
      }

      BossMan.bossKilled[name] = BossMan.bossKilled[name] || killed;
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
      button = "Shop";
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shop)
    {
      if (firstButton)
      {
        shop = this.shopName;
      }
    }
  }
}