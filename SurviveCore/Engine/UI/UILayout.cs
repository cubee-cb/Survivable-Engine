using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using SurviveCore.Engine.Display;
using SurviveCore.Engine.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.UI
{
  internal class UILayout : IdentifiableObject
  {
    public string id;

    [JsonIgnore] public UIProperties properties;
    [JsonIgnore] private Texture2D texture;
    [JsonIgnore] private Script lua;

    [JsonIgnore] private int t = 0;

    public UILayout(string id)
    {
      this.id = id;
      UpdateAssets();
    }

    public void UpdateAssets()
    {
      // set initial properties
      properties = Warehouse.GetJson<UIProperties>(id);

      // load assets
      texture = Warehouse.GetTexture(properties.texture);

      // initialise lua
      if (!string.IsNullOrWhiteSpace(properties.lua)) lua = Warehouse.GetLua(properties.lua);

    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="tick">current game tick</param>
    /// <param name="deltaTime">delta time of the previous frame</param>
    public virtual void Update(int tick, float deltaTime)
    {
      lua.Call(lua.Globals["Tick"], this);

      t += 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tickProgress">a value from 0-1 showing the progress through the current tick, for smoothing purposes</param>
    public virtual void Draw(Vector2 position, float tickProgress)
    {
      float localTick = t + tickProgress;

      // todo: handle spritesheets and multiple textures
      //spriteBatch.Draw(texture, visualPosition, Color.White);
      Rectangle clippingRect = new(0, 0, 128, 128);
      GameDisplay.Draw(texture, clippingRect, position);

    }


  }
}
