using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Rebirthing
{
  public class RebirthingStatsState : UIState
  {
    private RebirthingStatsPanel panel;
    public override void OnInitialize()
    {
      panel = new RebirthingStatsPanel();
      Append(panel);
    }
  }

  [Autoload(Side = ModSide.Client)]
  public class RebirthingStatsModSystem : ModSystem
  {

    private UserInterface ui;
    private RebirthingStatsState state;

    public bool IsOpen => this.ui.CurrentState != null;

    public void Show()
    {
      this.ui.SetState(state);
    }

    public void Hide()
    {
      this.ui.SetState(null);
    }

    public override void PostSetupContent()
    {
      // Create custom interface which can swap between different UIStates
      this.ui = new UserInterface();
      // Creating custom UIState
      this.state = new RebirthingStatsState();

      // Activate calls Initialize() on the UIState if not initialized, then calls OnActivate and then calls Activate on every child element
      this.state.Activate();
    }

    public override void UpdateUI(GameTime gameTime)
    {
      // Here we call .Update on our custom UI and propagate it to its state and underlying elements
      if (ui?.CurrentState != null)
      {
        ui?.Update(gameTime);
      }
    }

    // Adding a custom layer to the vanilla layer list that will call .Draw on your interface if it has a state
    // Setting the InterfaceScaleType to UI for appropriate UI scaling
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
      int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
      if (mouseTextIndex != -1)
      {
        layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
          "Rebirthing: Specs",
          delegate
          {
            if (this.ui?.CurrentState != null)
            {
              this.ui.Draw(Main.spriteBatch, new GameTime());
            }
            return true;
          },
          InterfaceScaleType.UI)
        );
      }
    }
  }

  public class RebirthingStatsPanel : UIPanel
  {
    private UIList list;

    public static RebirthingStatsPanel Instance { get; private set; }

    public override void OnInitialize()
    {
      this.Top.Percent = 0.20f;
      this.Left.Percent = 0.30f;
      this.Width.Percent = 0.30f;
      this.Height.Percent = 0.60f;

      UIText title = new UIText("Stats");
      this.Append(title);

      list = new UIList();
      list.Top.Pixels = 30;
      list.Width.Percent = 0.98f;
      list.Height.Percent = 1f;
      list.ListPadding = 5f;
      list.HAlign = 1f;
      list.ManualSortMethod = (list) => { };
      this.Append(list);

      UIScrollbar scrollbar = new UIScrollbar();
      scrollbar.Height.Percent = 1;
      scrollbar.HAlign = 1f;
      list.SetScrollbar(scrollbar);
      this.Append(scrollbar);

      this.Refresh();
    }

    public override void OnActivate()
    {
      Instance = this;

      this.Refresh();
    }

    public void Refresh()
    {
      this.list.Clear();

      if (Rebirthing.Player == null)
      {
        return;
      }

      RebirthPlayer rp = Rebirthing.Player;
      Player p = rp.Player;

      list.Add(new UIText($"Health: {p.statLifeMax} (base {rp.BaseStats["Health"]} +{rp.GetAttributeValue("Health")} x{1 + rp.GetTAttributeValue("Health")})"));
      list.Add(new UIText($"Health Regen: {p.lifeRegen} (base {rp.BaseStats["Health Regen"]} +{rp.GetAttributeValue("Health Regen")} x{1 + rp.GetTAttributeValue("Health Regen")})"));
      list.Add(new UIText($"Defense: {p.statDefense} (base {rp.BaseStats["Defense"]} +{rp.GetAttributeValue("Defense")} x{1 + rp.GetTAttributeValue("Defense")})"));
      
      list.Add(new UIHorizontalSeparator()
      {
        Left = new StyleDimension(0, 0.05f),
        Width = new StyleDimension(0, 0.8f)
      });

      list.Add(new UIText($"Mana: {p.statManaMax} (base {rp.BaseStats["Mana"]} +{rp.GetAttributeValue("Mana")} x{1 + rp.GetTAttributeValue("Mana")})"));
      list.Add(new UIText($"Mana Regen: {p.manaRegen} (base {rp.BaseStats["Mana Regen"]} +{rp.GetAttributeValue("Mana Regen")} x{1 + rp.GetTAttributeValue("Mana Regen")})"));
      list.Add(new UIText($"Mana Cost: {p.manaCost} (base {rp.BaseStats["Mana Reduction"]} -{rp.GetAttributeValue("Mana Reduction")} x{1 - rp.GetTAttributeValue("Mana Reduction")})"));

      list.Add(new UIHorizontalSeparator()
      {
        Left = new StyleDimension(0, 0.05f),
        Width = new StyleDimension(0, 0.8f)
      });

      list.Add(new UIText($"Damage: {p.GetDamage(DamageClass.Generic).ApplyTo(p.HeldItem?.damage ?? 0)} (base {p.HeldItem?.damage ?? 0} +{rp.GetAttributeValue("Damage")} x{1 + rp.GetTAttributeValue("Damage")})"));
      list.Add(new UIText($"Armor Pen: {p.GetArmorPenetration(DamageClass.Generic)} (base {rp.BaseStats["Armor Pen"]} +{rp.GetAttributeValue("Armor Pen")} x{1 + rp.GetTAttributeValue("Armor Pen")})"));
      list.Add(new UIText($"Crit Rate: {p.GetCritChance(DamageClass.Generic)} (base {rp.BaseStats["Crit Rate"]} +{rp.GetAttributeValue("Crit Rate")} x{1 + rp.GetTAttributeValue("Crit Rate")})"));
      list.Add(new UIText($"Crit Damage: x{(2 + rp.GetAttributeValue("Crit Damage")) * (1 + rp.GetTAttributeValue("Crit Damage"))} (base {2} +{rp.GetAttributeValue("Crit Damage")} x{1 + rp.GetTAttributeValue("Crit Damage")})"));
      list.Add(new UIText($"Attack Speed: {p.GetAttackSpeed(DamageClass.Generic)} (base {rp.BaseStats["Attack Speed"]} +{rp.GetAttributeValue("Attack Speed")} x{1 + rp.GetTAttributeValue("Attack Speed")})"));
      list.Add(new UIText($"Max Minions: {p.maxMinions} (base {rp.BaseStats["Max Minions"]} +{rp.GetAttributeValue("Max Minions")} x{1 + rp.GetTAttributeValue("Max Minions")})"));

      list.Add(new UIHorizontalSeparator()
      {
        Left = new StyleDimension(0, 0.05f),
        Width = new StyleDimension(0, 0.8f)
      });

      list.Add(new UIText($"Move Speed: {p.moveSpeed} (base {rp.BaseStats["Speed"]} +{rp.GetAttributeValue("Speed")} x{1 + rp.GetTAttributeValue("Speed")})"));
      list.Add(new UIText($"Block Break Time: {p.pickSpeed} (base {rp.BaseStats["Block Break Speed"]} -{rp.GetAttributeValue("Block Break Speed")} x{1 - rp.GetTAttributeValue("Block Break Speed")})"));
      list.Add(new UIText($"Reach: {rp.GetCurrentReach()} (base {rp.BaseStats["Reach"]} +{rp.GetAttributeValue("Reach")} x{1 + rp.GetTAttributeValue("Reach")})"));
      list.Add(new UIText($"Flight Time: {p.wingTimeMax} (base {rp.BaseStats["Flight"]} +{rp.GetAttributeValue("Flight")} x{1 + rp.GetTAttributeValue("Flight")})"));
      list.Add(new UIText($"Fishing Power: {p.fishingSkill} (base {rp.BaseStats["Fishing"]} +{rp.GetAttributeValue("Fishing")} +{rp.GetTAttributeValue("Fishing")})"));
      list.Add(new UIText($"Buff Duration: x{1 * (1 + rp.GetAttributeValue("Buff Duration")) * (1 + rp.GetTAttributeValue("Buff Duration"))} (base {1} x{1 + rp.GetAttributeValue("Buff Duration")} x{1 + rp.GetTAttributeValue("Buff Duration")})"));
    }

    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      // Checking ContainsPoint and then setting mouseInterface to true is very common
      // This causes clicks on this UIElement to not cause the player to use current items
      if (ContainsPoint(Main.MouseScreen))
      {
        Main.LocalPlayer.mouseInterface = true;
      }

      // Here we check if the DraggableUIPanel is outside the Parent UIElement rectangle
      // (In our example, the parent would be ExampleCoinsUI, a UIState. This means that we are checking that the DraggableUIPanel is outside the whole screen)
      // By doing this and some simple math, we can snap the panel back on screen if the user resizes his window or otherwise changes resolution
      var parentSpace = Parent.GetDimensions().ToRectangle();
      if (!GetDimensions().ToRectangle().Intersects(parentSpace))
      {
        Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
        Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
        // Recalculate forces the UI system to do the positioning math again.
        Recalculate();
      }
    }

  }
}