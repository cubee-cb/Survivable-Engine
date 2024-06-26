using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using SurviveCore.Engine.Display;
using SurviveCore.Engine.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Items
{
  internal class Item : IdentifiableObject
  {

    [JsonIgnore] public ItemProperties properties;
    [JsonIgnore] private readonly Texture2D texture;
    [JsonIgnore] private readonly Script lua;

    [JsonIgnore] private int t = 0;

    [JsonIgnore] private int currentFrame = 0;
    [JsonIgnore] private int frameWidth = 16;
    [JsonIgnore] private int frameHeight = 16;

    public Item(string id)
    {

      // set initial properties
      properties = Warehouse.GetJson<ItemProperties>(id);

      // load assets
      texture = Warehouse.GetTexture(properties.textureSheetName);
      if (properties.sounds != null)
      {
        foreach (string fileName in properties.sounds)
        {
          Warehouse.GetSoundEffect(fileName);
        }
      }

      frameWidth = texture.Width / (int)MathF.Max(properties.framesX, 1);
      frameHeight = texture.Height / (int)MathF.Max(properties.framesY, 1);

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
      //luaTick.Call(luaTick.Globals["update"], this);

      t += 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tickProgress">a value from 0-1 showing the progress through the current tick, for smoothing purposes</param>
    public virtual void Draw(Vector2 position, float tickProgress)
    {
      // todo: handle spritesheets and multiple textures
      //spriteBatch.Draw(texture, visualPosition, Color.White);
      Rectangle clippingRect = new(frameWidth * (t / 10 % properties.framesX), frameHeight * currentFrame, frameWidth, frameHeight);
      GameDisplay.Draw(texture, clippingRect, position);

    }


  }
}
