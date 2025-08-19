using System.Collections.Generic;

namespace Rebirthing
{
  public class RebirthAttribute
  {
    public string Id { get; set; }
    public int Level { get; set; }

    public RebirthAttribute Clone()
    {
      return new RebirthAttribute()
      {
        Id = this.Id,
        Level = this.Level
      };
    }

    public static List<string> List = new List<string>()
    {
      "Health",
      "Health Regen",
      "Defense",
      "Mana",
      "Mana Regen",
      "Mana Reduction",
      "Speed",
      "Flight",
      "Damage",
      "Armor Pen",
      "Attack Speed",
      "Crit Rate",
      "Crit Damage",
      "Max Minions",
      "Block Break Speed",
      "Buff Duration",
      "Reach",
      "Fishing",
    };
  }
}