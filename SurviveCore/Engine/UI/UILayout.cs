using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using SurviveCore.Engine.Display;
using SurviveCore.Engine.Items;
using SurviveCore.Engine.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.UI
{
  [MoonSharpUserData]
  internal class UILayout : IdentifiableObject
  {
    public string id;
    public Inventory associatedInventory;

    [JsonIgnore] public UIProperties properties;
    [JsonIgnore] private Texture2D texture;
    [JsonIgnore] private Script lua;

    [JsonIgnore] private int t = 0;

    public UILayout(string id, Inventory associatedInventory)
    {
      this.id = id;
      this.associatedInventory = associatedInventory;
      UpdateAssets();
    }

    public void UpdateAssets()
    {
      // set initial properties
      properties = Warehouse.GetJson<UIProperties>(id);

      // load assets
      texture = Warehouse.GetTexture(properties.texture);

      // initialise lua
      if (!string.IsNullOrWhiteSpace(properties.lua))
      {
        lua = Warehouse.GetLua(properties.lua);

        // pass methods to lua
        //lua.Globals["Move"] = (Func<float, float, float, bool>)Move;
      }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="tick">current game tick</param>
    /// <param name="deltaTime">delta time of the previous frame</param>
    public virtual void Update(int tick, float deltaTime)
    {
      if (lua != null)
      {
        //if (lua.Globals["Tick"] != null) lua.Call(lua.Globals["Tick"], this);

        try
        {
          DynValue resTick;
          if (lua.Globals["Tick"] != null) resTick = lua.Call(lua.Globals["Tick"]);
          
        }
        catch (Exception e)
        {
          ELDebug.Log("LUA error: \n" + e);
        }

      }

      t += 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position">where on the screen to draw the ui, pixels</param>
    /// <param name="tickProgress">a value from 0-1 showing the progress through the current tick, for smoothing purposes</param>
    public virtual void Draw(Vector2 position, float tickProgress)
    {
      float localTick = t + tickProgress;

      // todo: handle spritesheets and multiple textures
      //spriteBatch.Draw(texture, visualPosition, Color.White);

      if (properties.widgets != null)
      {
        foreach (UIWidget widget in properties.widgets)
        {
          Vector2 widgetPosition = new(widget.position[0], widget.position[1]);

          Rectangle clippingRect = new(0, 24, 24, 24);
          GameDisplay.Draw(texture, clippingRect, position + widgetPosition * clippingRect.Size.ToVector2());
        }
      }

      if (properties.slots != null)
      {
        int index = 0;
        foreach (UISlot slot in properties.slots)
        {
          // grab item from the inventory
          List<Item> items = associatedInventory.GetItems();

          // break if there are no more item slots in the inventory
          if (index >= items.Count) break;
          Item item = items[index];

          Vector2 slotPosition = new(slot.position[0], slot.position[1]);

          Rectangle clippingRect = new(0, 0, 24, 24);
          GameDisplay.Draw(texture, clippingRect, position + slotPosition * clippingRect.Size.ToVector2(), depth: 1);

          if (item != null)
          {
            item.Draw(position + slotPosition * clippingRect.Size.ToVector2() + clippingRect.Size.ToVector2() / 2f, tickProgress);
          }

          index++;
        }
      }

    }


    //                           //
    // Lua-exposed methods below //
    // v v v v v v v v v v v v v //


    /// <summary>
    /// Gets the distance from the mob to the specified coordinates.
    /// </summary>
    /// <param name="x">Target X position.</param>
    /// <param name="y">Target X position.</param>
    /// <returns>The distance to the target.</returns>
    private Table GetSlot(int index)
    {
      Item item = associatedInventory.GetItems()[index];

      Table table = new(lua);
      table.Set("id", DynValue.NewString(item.id));
      table.Set("amount", DynValue.NewNumber(0));

      return table;
    }


  }
}
