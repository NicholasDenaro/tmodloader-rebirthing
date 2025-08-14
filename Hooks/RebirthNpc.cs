using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Rebirthing
{
  public class RebirthNpc : GlobalNPC
  {

    public override void OnKill(NPC npc)
    {
      Rebirthing.Instance.AwardKillExp(npc.whoAmI);
      npc.value *= Rebirthing.CoinRate;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
      foreach (IItemDropRule dropRule in npcLoot.Get())
      {
        if (dropRule is DropBasedOnExpertMode drop && drop.ruleForNormalMode is CommonDrop normalDropRule)
        {
          normalDropRule.chanceNumerator = (int)(normalDropRule.chanceNumerator * Rebirthing.DropRate);
          if (new Item(normalDropRule.itemId).maxStack != 1)
          {
            normalDropRule.amountDroppedMinimum = Math.Max(Math.Min(normalDropRule.amountDroppedMinimum, 1), (int)(normalDropRule.amountDroppedMinimum * Rebirthing.DropCountRate));
            normalDropRule.amountDroppedMaximum = Math.Max(Math.Min(normalDropRule.amountDroppedMaximum, 1), (int)(normalDropRule.amountDroppedMaximum * Rebirthing.DropCountRate));
          }
        }
      }
    }
  }
}