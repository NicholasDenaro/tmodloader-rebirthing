using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Rebirthing.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Rebirthing.Npcs
{
  [AutoloadHead]
  public class BossMan : ModNPC
  {

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

    public override string GetChat()
    {
      return "I provide items to spawn bosses.";
    }

    public override bool CanTownNPCSpawn(int numTownNPCs)
    {
      this.UpdatingDefeatedBosses();

      return Rebirthing.BossKilled.Keys.Any(key => Rebirthing.BossKilled[key]);
    }

    public override bool PreAI()
    {
      this.UpdatingDefeatedBosses();

      return true;
    }

    private void UpdatingDefeatedBosses()
    {
      this.AddOrUpdateBoss("downedAncientCultist", NPC.downedAncientCultist); // done
      this.AddOrUpdateBoss("downedBoss1", NPC.downedBoss1); // done
      this.AddOrUpdateBoss("downedBoss2", NPC.downedBoss2); // done
      this.AddOrUpdateBoss("downedBoss3", NPC.downedBoss3); // done
      this.AddOrUpdateBoss("downedChristmasIceQueen", NPC.downedChristmasIceQueen); // skip
      this.AddOrUpdateBoss("downedChristmasSantank", NPC.downedChristmasSantank); // skip
      this.AddOrUpdateBoss("downedChristmasTree", NPC.downedChristmasTree); // skip
      this.AddOrUpdateBoss("downedDeerclops", NPC.downedDeerclops); // done
      this.AddOrUpdateBoss("downedEmpressOfLight", NPC.downedEmpressOfLight); // done
      this.AddOrUpdateBoss("downedFishron", NPC.downedFishron); // done
      this.AddOrUpdateBoss("downedFrost", NPC.downedFrost); // done
      this.AddOrUpdateBoss("downedGoblins", NPC.downedGoblins); // done
      this.AddOrUpdateBoss("downedGolemBoss", NPC.downedGolemBoss); // done
      this.AddOrUpdateBoss("downedHalloweenKing", NPC.downedHalloweenKing); // done
      this.AddOrUpdateBoss("downedHalloweenTree", NPC.downedHalloweenTree); // skip
      this.AddOrUpdateBoss("downedMartians", NPC.downedMartians); // done
      this.AddOrUpdateBoss("downedMechBoss1", NPC.downedMechBoss1); // done
      this.AddOrUpdateBoss("downedMechBoss2", NPC.downedMechBoss2); // done
      this.AddOrUpdateBoss("downedMechBoss3", NPC.downedMechBoss3); // done
      this.AddOrUpdateBoss("downedMoonlord", NPC.downedMoonlord); // done
      this.AddOrUpdateBoss("downedPirates", NPC.downedPirates); // done
      this.AddOrUpdateBoss("downedPlantBoss", NPC.downedPlantBoss); // done
      this.AddOrUpdateBoss("downedQueenBee", NPC.downedQueenBee); // done
      this.AddOrUpdateBoss("downedQueenSlime", NPC.downedQueenSlime); // done
      this.AddOrUpdateBoss("downedSlimeKing", NPC.downedSlimeKing); // done
      this.AddOrUpdateBoss("downedTowerNebula", NPC.downedTowerNebula);
      this.AddOrUpdateBoss("downedTowerSolar", NPC.downedTowerSolar);
      this.AddOrUpdateBoss("downedTowerStardust", NPC.downedTowerStardust);
      this.AddOrUpdateBoss("downedTowerVortex", NPC.downedTowerVortex);
      this.AddOrUpdateBoss("hardMode", Main.hardMode); // done
      this.AddOrUpdateBoss("bloodMoon", Main.bloodMoon); // done
      this.AddOrUpdateBoss("solarEclipse", Main.eclipse); // done
    }

    public override void AddShops()
    {
      NPCShop shop = new NPCShop(NPC.type, shopName);

      shop.Add(new NPCShop.Entry(ItemID.SuspiciousLookingEye, new Condition("downedBoss1", () => Rebirthing.BossKilled.GetValueOrDefault("downedBoss1", false))));

      if (WorldGen.crimson)
      {
        shop.Add(new NPCShop.Entry(ItemID.BloodySpine, new Condition("downedBoss2", () => Rebirthing.BossKilled.GetValueOrDefault("downedBoss2", false))));
      }
      else
      {
        shop.Add(new NPCShop.Entry(ItemID.WormFood, new Condition("downedBoss2", () => Rebirthing.BossKilled.GetValueOrDefault("downedBoss2", false))));
      }

      shop.Add<SkeletronSummon>(new Condition("downedBoss3", () => Rebirthing.BossKilled.GetValueOrDefault("downedBoss3", false)));

      // Pre Hardmode Optionals
      shop.Add<DeerclopsSummon>(new Condition("downedDeerclops", () => Rebirthing.BossKilled.GetValueOrDefault("downedDeerclops", false)));
      shop.Add(new NPCShop.Entry(ItemID.Abeemination, new Condition("downedQueenBee", () => Rebirthing.BossKilled.GetValueOrDefault("downedQueenBee", false))));
      shop.Add(new NPCShop.Entry(ItemID.SlimeCrown, new Condition("downedSlimeKing", () => Rebirthing.BossKilled.GetValueOrDefault("downedSlimeKing", false))));

      // Hardmode+
      shop.Add<WOFSummon>(new Condition("hardMode", () => Rebirthing.BossKilled.GetValueOrDefault("hardMode", false)));
      shop.Add(new NPCShop.Entry(ItemID.MechanicalWorm, new Condition("downedMechBoss1", () => Rebirthing.BossKilled.GetValueOrDefault("downedMechBoss1", false))));
      shop.Add(new NPCShop.Entry(ItemID.MechanicalEye, new Condition("downedMechBoss2", () => Rebirthing.BossKilled.GetValueOrDefault("downedMechBoss2", false))));
      shop.Add(new NPCShop.Entry(ItemID.MechanicalSkull, new Condition("downedMechBoss3", () => Rebirthing.BossKilled.GetValueOrDefault("downedMechBoss3", false))));

      // Plantera+
      shop.Add<PlanteraSummon>(new Condition("downedPlantBoss", () => Rebirthing.BossKilled.GetValueOrDefault("downedPlantBoss", false)));
      shop.Add(new NPCShop.Entry(ItemID.LihzahrdPowerCell, new Condition("downedGolemBoss", () => Rebirthing.BossKilled.GetValueOrDefault("downedGolemBoss", false))));
      shop.Add<CultistSummon>(new Condition("downedAncientCultist", () => Rebirthing.BossKilled.GetValueOrDefault("downedAncientCultist", false)));

      shop.Add<NebulaSummon>(new Condition("downedTowerNebula", () => Rebirthing.BossKilled.GetValueOrDefault("downedTowerNebula", false)));
      shop.Add<SolarSummon>(new Condition("downedTowerSolar", () => Rebirthing.BossKilled.GetValueOrDefault("downedTowerSolar", false)));
      shop.Add<StardustSummon>(new Condition("downedTowerStardust", () => Rebirthing.BossKilled.GetValueOrDefault("downedTowerStardust", false)));
      shop.Add<VortexSummon>(new Condition("downedTowerVortex", () => Rebirthing.BossKilled.GetValueOrDefault("downedTowerVortex", false)));


      shop.Add(new NPCShop.Entry(ItemID.CelestialSigil, new Condition("downedMoonlord", () => Rebirthing.BossKilled.GetValueOrDefault("downedMoonlord", false))));

      // Hardmode Optionals
      shop.Add(new NPCShop.Entry(ItemID.QueenSlimeCrystal, new Condition("downedQueenSlime", () => Rebirthing.BossKilled.GetValueOrDefault("downedQueenSlime", false))));
      shop.Add(new NPCShop.Entry(ItemID.EmpressButterfly, new Condition("downedEmpressOfLight", () => Rebirthing.BossKilled.GetValueOrDefault("downedEmpressOfLight", false))));
      shop.Add(new NPCShop.Entry(ItemID.TruffleWorm, new Condition("downedFishron", () => Rebirthing.BossKilled.GetValueOrDefault("downedFishron", false))));

      // Events
      shop.Add(new NPCShop.Entry(ItemID.BloodMoonStarter, new Condition("bloodMoon", () => Rebirthing.BossKilled.GetValueOrDefault("bloodMoon", false))));
      shop.Add(new NPCShop.Entry(ItemID.GoblinBattleStandard, new Condition("downedGoblins", () => Rebirthing.BossKilled.GetValueOrDefault("downedGoblins", false))));

      shop.Add(new NPCShop.Entry(ItemID.SolarTablet, new Condition("solarEclipse", () => Rebirthing.BossKilled.GetValueOrDefault("solarEclipse", false))));
      shop.Add(new NPCShop.Entry(ItemID.PirateMap, new Condition("downedPirates", () => Rebirthing.BossKilled.GetValueOrDefault("downedPirates", false))));
      shop.Add<MartianSummon>(new Condition("downedMartians", () => Rebirthing.BossKilled.GetValueOrDefault("downedMartians", false)));
      shop.Add(new NPCShop.Entry(ItemID.SnowGlobe, new Condition("downedFrost", () => Rebirthing.BossKilled.GetValueOrDefault("downedFrost", false) || Rebirthing.BossKilled.GetValueOrDefault("downedMoonlord", false)))); // Since it's time gated, we give an option to unlock it after moonlord
      shop.Add(new NPCShop.Entry(ItemID.PumpkinMoonMedallion, new Condition("downedHalloweenKing", () => Rebirthing.BossKilled.GetValueOrDefault("downedHalloweenKing", false))));

      shop.Register();
    }

    private void AddOrUpdateBoss(string name, bool killed)
    {
      if (!Rebirthing.BossKilled.ContainsKey(name))
      {
        Rebirthing.BossKilled.Add(name, killed);
      }

      Rebirthing.BossKilled[name] = Rebirthing.BossKilled[name] || killed;
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