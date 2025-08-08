using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Rebirthing
{
  public class RebirthingSpecsState : UIState
  {
    private RebirthingSpecsPanel panel;
    public override void OnInitialize()
    {
      panel = new RebirthingSpecsPanel();
      Append(panel);
    }

    public void SetType(string type)
    {
      this.panel.SetType(type);
    }
  }

  [Autoload(Side = ModSide.Client)]
  public class RebirthingSpecsSystem : ModSystem
  {
    private UserInterface ui;
    private RebirthingSpecsState state;

    public bool IsOpen => this.ui.CurrentState != null;

    public void Show(string type)
    {
      this.state.SetType(type);
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
      this.state = new RebirthingSpecsState();

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

  public class RebirthingSpecsPanel : UIPanel
  {
    private string selectedText;
    // Stores the offset from the top left of the UIPanel while dragging
    private Vector2 offset;
    // A flag that checks if the panel is currently being dragged
    private bool dragging;

    private UIPanel treePanel;

    private UIList descriptionList;

    private UIText totalLevelText;

    private UIText attributesText;

    private string selectedSpec;

    private string SpecType = "Rebirth";

    public void SetType(string type)
    {
      this.SpecType = type;
    }

    public override void OnInitialize()
    {
      this.Top.Percent = 0.20f;
      this.Left.Percent = 0.30f;
      this.Width.Percent = 0.30f;
      this.Height.Percent = 0.60f;

      totalLevelText = new UIText("Total Level: 0");
      totalLevelText.Left.Percent = 0.01f;
      this.Append(totalLevelText);

      attributesText = new UIText($"{SpecType} energy: 0");
      attributesText.Left.Percent = 0.42f;
      this.Append(attributesText);

      UIButton<string> respec = new UIButton<string>("Respec")
      {
        MaxWidth = { Percent = 0.95f },
        HAlign = 1.0f,
        ScalePanel = true,
        AltPanelColor = UICommon.MainPanelBackground,
        AltHoverPanelColor = UICommon.MainPanelBackground * (1 / 0.8f)
      };
      respec.OnLeftClick += (_, _) =>
      {
        SoundEngine.PlaySound(SoundID.MenuTick);
        if (this.SpecType == "Rebirth")
        {
          Rebirthing.Player.Respec();
        }
        else if (this.SpecType == "Transcendence")
        {
          Rebirthing.Player.RespecTranscendance();
        }
        this.SetDescription(this.selectedSpec);

        this.UpdateAmounts();
      };
      this.Append(respec);

      this.treePanel = new UIPanel();
      this.treePanel.Left.Percent = 0.01f;
      this.treePanel.Top.Percent = 0.11f;
      this.treePanel.Width.Percent = 0.40f;
      this.treePanel.Height.Percent = 0.88f;
      this.Append(treePanel);

      UIList list = new UIList();
      list.Width.Percent = 0.98f;
      list.Height.Percent = 1f;
      list.ListPadding = 5f;
      list.HAlign = 1f;
      list.ManualSortMethod = (list) => { };
      this.AddAttributes(list);
      this.treePanel.Append(list);

      UIScrollbar scrollbar = new UIScrollbar();
      scrollbar.Height.Percent = 1;
      scrollbar.HAlign = 1f;
      list.SetScrollbar(scrollbar);
      this.treePanel.Append(scrollbar);

      UITextPanel<string> descriptionPanel = new UITextPanel<string>("Description");
      descriptionPanel.Left.Percent = 0.42f;
      descriptionPanel.Width.Percent = 0.57f;
      descriptionPanel.Top.Percent = 0.11f;
      descriptionPanel.Height.Percent = 0.88f;
      this.Append(descriptionPanel);

      this.descriptionList = new UIList();
      this.descriptionList.Width.Percent = 0.9f;
      this.descriptionList.Height.Percent = 0.9f;
      this.descriptionList.ListPadding = 5f;
      this.descriptionList.ManualSortMethod = (list) => { };
      this.descriptionList.Add(new UIText("hi"));
      descriptionPanel.Append(this.descriptionList);
    }

    public override void OnActivate()
    {
      this.UpdateAmounts();
    }

    private void UpdateAmounts()
    {
      if (Rebirthing.Player != null)
      {
        if (this.SpecType == "Rebirth")
        {
          this.totalLevelText.SetText("Total Level: " + Rebirthing.Player.RebirthData.TotalLevel);
          this.attributesText.SetText("Rebirth energy: " + Rebirthing.Player.RebirthData.RebirthPoints);
        }
        else if (this.SpecType == "Transcendence")
        {
          this.totalLevelText.SetText("Transendence Level: " + Rebirthing.Player.RebirthData.TranscendenceLevel);
          this.attributesText.SetText("Transcendence energy: " + Rebirthing.Player.RebirthData.TranscendencePoints);
        }
      }
    }

    private void AddAttributes(UIList list)
    {
      foreach (string attr in RebirthAttribute.List)
      {
        list.Add(SpecOption(attr));
      }
    }

    private UIButton<string> SpecOption(string text)
    {
      UIButton<string> button = new UIButton<string>(text)
      {
        MaxWidth = { Percent = 0.95f },
        HAlign = 0.0f,
        ScalePanel = true,
        AltPanelColor = UICommon.MainPanelBackground,
        AltHoverPanelColor = UICommon.MainPanelBackground * (1 / 0.8f),
        UseAltColors = () => selectedText != text,
      }.WithFadedMouseOver();

      button.OnLeftClick += (a, b) =>
      {
        SoundEngine.PlaySound(SoundID.MenuOpen);
        this.SetDescription(text);
      };

      return button;
    }

    private void SetDescription(string text)
    {
      if (this.SpecType == "Rebirth")
      {
        this.SetRebirthDescription(text);
      }
      else if (this.SpecType == "Transcendence")
      {
        this.SetTranscendenceDescription(text);
      }
    }

    private void SetRebirthDescription(string text)
    {
      if (!RebirthAttribute.List.Contains(text))
      {
        return;
      }

      this.selectedSpec = text;
      RebirthAttribute attr = Rebirthing.Player.GetAttribute(text);

      this.descriptionList.Clear();
      this.descriptionList.Add(new UIText(""));
      UIText title = new UIText(text);
      this.descriptionList.Add(title);
      UIText level = new UIText("Level " + attr.Level);
      this.descriptionList.Add(level);
      UIText description = new UIText(Language.GetTextValue($"Mods.Rebirthing.Attributes.{text}.{SpecType}.Description"));
      this.descriptionList.Add(description);

      UIText current = new UIText("Current value: " + Rebirthing.Player.GetAttributeValue(text));
      this.descriptionList.Add(current);

      int cost = Rebirthing.Player.GetAttributeCost(text);
      this.descriptionList.Add(new UIText("Cost: " + cost));
      UIButton<string> upgrade = new UIButton<string>("Upgrade")
      {
        MaxWidth = { Percent = 0.95f },
        HAlign = 0.0f,
        ScalePanel = true,
        AltPanelColor = UICommon.MainPanelBackground,
        AltHoverPanelColor = UICommon.MainPanelBackground * (1 / 0.8f),
        UseAltColors = () => Rebirthing.Player.RebirthData.RebirthPoints >= cost,
      }.WithFadedMouseOver(); ;
      upgrade.OnLeftClick += (_, _) =>
      {
        if (Rebirthing.Player.RebirthData.RebirthPoints >= cost)
        {
          Rebirthing.Player.RebirthData.RebirthPoints -= cost;
          if (!Rebirthing.Player.RebirthData.RebirthAttributes.ContainsKey(text))
          {
            Rebirthing.Player.RebirthData.RebirthAttributes[text] = attr;
          }

          attr.Level++;
          SetDescription(text);

          this.UpdateAmounts();
        }
      };

      this.descriptionList.Add(upgrade);
    }

    private void SetTranscendenceDescription(string text)
    {
      if (!RebirthAttribute.List.Contains(text))
      {
        return;
      }

      this.selectedSpec = text;
      RebirthAttribute attr = Rebirthing.Player.GetTAttribute(text);

      this.descriptionList.Clear();
      this.descriptionList.Add(new UIText(""));
      UIText title = new UIText(text);
      this.descriptionList.Add(title);
      UIText level = new UIText("Level " + attr.Level);
      this.descriptionList.Add(level);
      UIText description = new UIText(Language.GetTextValue($"Mods.Rebirthing.Attributes.{text}.{SpecType}.Description"));
      this.descriptionList.Add(description);

      UIText current = new UIText("Current value: " + Rebirthing.Player.GetTAttributeValue(text));
      this.descriptionList.Add(current);

      int cost = Rebirthing.Player.GetTAttributeCost(text);
      this.descriptionList.Add(new UIText("Cost: " + cost));
      UIButton<string> upgrade = new UIButton<string>("Upgrade")
      {
        MaxWidth = { Percent = 0.95f },
        HAlign = 0.0f,
        ScalePanel = true,
        AltPanelColor = UICommon.MainPanelBackground,
        AltHoverPanelColor = UICommon.MainPanelBackground * (1 / 0.8f),
        UseAltColors = () => Rebirthing.Player.RebirthData.RebirthPoints >= cost,
      }.WithFadedMouseOver(); ;
      upgrade.OnLeftClick += (_, _) =>
      {
        if (Rebirthing.Player.RebirthData.TranscendencePoints >= cost)
        {
          Rebirthing.Player.RebirthData.TranscendencePoints -= cost;
          if (!Rebirthing.Player.RebirthData.TranscendenceAttributes.ContainsKey(text))
          {
            Rebirthing.Player.RebirthData.TranscendenceAttributes[text] = attr;
          }

          attr.Level++;
          SetDescription(text);

          this.UpdateAmounts();
        }
      };

      this.descriptionList.Add(upgrade);
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