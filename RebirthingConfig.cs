using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Rebirthing
{
  public class RebirthingClientConfig : ModConfig
  {
    public override ConfigScope Mode => ConfigScope.ClientSide;
  }

  public class RebirthingServerConfig : ModConfig
  {
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [DefaultValue("1.0")]
    public string ExpRate;

    [DefaultValue("1.0")]
    public string CoinRate;

    [DefaultValue("1.0")]
    public string SpecsRate;

    [DefaultValue("1.0")]
    public string DropRate;

    [DefaultValue("1.0")]
    public string DropCountRate; 

    public override void OnLoaded()
    {
      Rebirthing.ExpRate = float.Parse(this.ExpRate);
      Rebirthing.CoinRate = float.Parse(this.CoinRate);
      Rebirthing.SpecsRate = float.Parse(this.SpecsRate);
      Rebirthing.DropRate = float.Parse(this.DropRate);
      Rebirthing.DropCountRate = float.Parse(this.DropCountRate);
    }

    public override void OnChanged()
    {
      Rebirthing.ExpRate = float.Parse(this.ExpRate);
      Rebirthing.CoinRate = float.Parse(this.CoinRate);
      Rebirthing.SpecsRate = float.Parse(this.SpecsRate);
      Rebirthing.DropRate = float.Parse(this.DropRate);
      Rebirthing.DropCountRate = float.Parse(this.DropCountRate);
    }
  }
}