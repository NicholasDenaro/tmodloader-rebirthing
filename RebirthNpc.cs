using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Rebirthing
{
  public class RebirthNpc : GlobalNPC
  {

    public override void OnKill(NPC npc)
    {
      Rebirthing.Instance.AwardExp(npc.whoAmI);
      npc.value *= Rebirthing.CoinRate;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
      // Doesn't seem to work
      // foreach (IItemDropRule dropRule in npcLoot.Get())
      // {
      //   Rebirthing.Write("drop rule" + dropRule);
      //   if (dropRule is DropBasedOnExpertMode drop && drop.ruleForNormalMode is CommonDrop normalDropRule)
      //   {
      //     normalDropRule.chanceNumerator = (int)(normalDropRule.chanceNumerator * Rebirthing.DropRate);
      //     normalDropRule.amountDroppedMinimum = Math.Max(Math.Min(normalDropRule.amountDroppedMinimum, 1), (int)(normalDropRule.amountDroppedMinimum * Rebirthing.DropCountRate));
      //     normalDropRule.amountDroppedMaximum = Math.Max(Math.Min(normalDropRule.amountDroppedMaximum, 1), (int)(normalDropRule.amountDroppedMaximum * Rebirthing.DropCountRate));
      //   }
      // }
    }

  }

  [AutoloadHead]
  public class Rebirther : ModNPC
  {

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

    public override bool CanTownNPCSpawn(int numTownNPCs)
    {
      return true;
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
      button = "Rebirth";
      button2 = "Spec";
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
      if (firstButton)
      {
        if (Rebirthing.Player.RebirthData.Level >= 25)
        {
          Rebirthing.Player.Rebirth();
          Main.npcChatText = String.Empty;
          // Closing the chat on the same frame is causing an index out of bounds excpetion within Main
          Main.RunOnMainThread(() =>
          {
            Main.CloseNPCChatOrSign();
          });
        }
        else
        {
          Main.npcChatText = "Talk to me again when you are at least level 25";
        }

      }
      else
      {
        Main.npcChatText = String.Empty;
        // Closing the chat on the same frame is causing an index out of bounds excpetion within Main
        Main.RunOnMainThread(() =>
        {
          Main.CloseNPCChatOrSign();
        });
        Main.playerInventory = true;
        ModContent.GetInstance<RebirthingSpecsSystem>().Show("Rebirth");
      }
    }
  }

  [AutoloadHead]
  public class Transcender : ModNPC
  {

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

    public override bool CanTownNPCSpawn(int numTownNPCs)
    {
      return Rebirthing.Players.Any(player => player.RebirthData.TotalLevel >= 50 || player.RebirthData.TranscendenceLevel > 0);
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
      button = "Transcend";
      button2 = "Spec";
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
      if (firstButton)
      {
        if (Rebirthing.Player.RebirthData.TotalLevel >= 50)
        {
          Rebirthing.Player.Transcend();
          Main.npcChatText = String.Empty;
          // Closing the chat on the same frame is causing an index out of bounds excpetion within Main
          Main.RunOnMainThread(() =>
          {
            Main.CloseNPCChatOrSign();
          });
        }
        else
        {
          Main.npcChatText = "Talk to me again when you are at least Total level 50";
        }

      }
      else
      {
        Main.npcChatText = String.Empty;
        // Closing the chat on the same frame is causing an index out of bounds excpetion within Main
        Main.RunOnMainThread(() =>
        {
          Main.CloseNPCChatOrSign();
        });
        Main.playerInventory = true;
        ModContent.GetInstance<RebirthingSpecsSystem>().Show("Transcendence");
      }
    }
  }

  [AutoloadHead]
  public class TerraLord : ModNPC
  {
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

    public override bool CanTownNPCSpawn(int numTownNPCs)
    {
      return NPC.downedMoonlord;
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
      button = "Reset World";
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
      if (firstButton)
      {
        // TODO reset all the boss flags?
        Rebirthing.Instance.IncrementWorld();
      }
    }
  }
}