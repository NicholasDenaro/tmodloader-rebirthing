using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.Config;

namespace Rebirthing
{
  public class PlayerData
  {
    public int Rebirths { get; set; } = 0;

    public Dictionary<string, RebirthAttribute> RebirthAttributes { get; set; } = new Dictionary<string, RebirthAttribute>();

    public Dictionary<string, RebirthAttribute> TranscendenceAttributes { get; set; } = new Dictionary<string, RebirthAttribute>();

    public int TotalLevel { get; set; } = 1;

    public int TranscendenceLevel { get; set; } = 1;

    public int Level { get; set; } = 1;

    public int Exp { get; set; } = 0;

    public int RebirthPoints { get; set; } = 0;

    public int TranscendencePoints { get; set; } = 0;

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
      int points = (int)(this.Level * Rebirthing.SpecsRate);
      this.RebirthPoints += points;
      Rebirthing.Write("Gained " + points + " rebirth energy");
      this.Level = 1;
      this.Exp = 0;
    }

    public void Respec()
    {
      this.RebirthPoints = (int)((this.TotalLevel - this.Level) * Rebirthing.SpecsRate);
      Rebirthing.Write("Reclaimed " + this.RebirthPoints + " rebirth energy");
      this.RebirthAttributes.Clear();
    }

    public void Transend()
    {
      int levels = (int)(this.TotalLevel * 0.9f);
      Rebirthing.Write("Sacrificed " + levels + " levels");
      this.TotalLevel -= levels;
      this.TranscendenceLevel += levels;
      this.TranscendencePoints += levels / 5;
      Rebirthing.Write("Gained " + (levels / 5) + " transcendence energy");
      this.Level = 1;
      this.RebirthPoints = (int)(this.TotalLevel * Rebirthing.SpecsRate);
      this.RebirthAttributes.Clear();
    }

    public void RespecTranscendance()
    {
      this.TranscendencePoints = this.TranscendenceLevel / 5;
      Rebirthing.Write("Reclaimed " + this.TranscendencePoints + " transcendence energy");
      this.TranscendenceAttributes.Clear();
    }
  }
}