using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.Config;

namespace Rebirthing
{
  public class PlayerData
  {
    public int Rebirths { get; set; } = 0;

    public Dictionary<string, RebirthAttribute> Attributes { get; set; } = new Dictionary<string, RebirthAttribute>();

    public int TotalLevel { get; set; } = 1;

    public int Level { get; set; } = 1;

    public int Exp { get; set; } = 0;

    public int AttributePoints { get; set; } = 0;

    public static int ExpPerLevel(int level)
    {
      return 20 + (int)Math.Pow(5, 1 + (level - 1.0) / 10);
    }

    public void Levelup()
    {
      this.Level++;
      this.TotalLevel++;
    }

    public int AddExP(int exp)
    {
      if (exp <= 0)
      {
        return 0;
      }

      int levels = 0;

      // Main.NewText("Gained " + exp + " exp");
      this.Exp += exp;
      int neededExp;
      while (this.Exp >= (neededExp = ExpPerLevel(this.Level)))
      {
        this.Exp -= neededExp;
        this.Levelup();
        levels++;
      }
      // Main.NewText("+" + exp + " exp " + this.Exp + "/" + neededExp + " (" + (((int)(this.Exp * 1000.0 / neededExp)) / 10) + "%)");

      return levels;
    }

    public void Rebirth()
    {
      this.Rebirths++;
      int points = (int)(Math.Log(Math.Max((this.Level - 20) / 5, 1)) / Math.Log(20) * 100 * Rebirthing.SpecsRate);
      this.AttributePoints += points;
      Rebirthing.Write("Gained " + points + " rebirth energy");
      this.Level = 1;
      this.Exp = 0;
    }
  }
}