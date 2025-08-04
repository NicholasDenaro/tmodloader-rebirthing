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

    public override void OnLoaded()
    {
      Rebirthing.ExpRate = float.Parse(this.ExpRate);
      Rebirthing.CoinRate = float.Parse(this.CoinRate);
    }

    public override void OnChanged()
    {
      Rebirthing.ExpRate = float.Parse(this.ExpRate);
      Rebirthing.CoinRate = float.Parse(this.CoinRate);
    }
  }
}