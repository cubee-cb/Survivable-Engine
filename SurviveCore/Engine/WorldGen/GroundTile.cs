using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using SurviveCore.Engine.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.WorldGen
{
  internal class GroundTile
  {
    private const int TILE_THICKNESS = 16;

    private int elevation = 0;

    [JsonIgnore] GroundProperties properties;
    [JsonIgnore] private readonly Texture2D texture;
    [JsonIgnore] private readonly Script lua;

    public GroundTile(string id, int elevation)
    {
      properties = Warehouse.GetJson<GroundProperties>(id);

      texture = Warehouse.GetTexture(properties.textureSheetName);

      this.elevation = elevation;

      // initialise lua
      if (!string.IsNullOrWhiteSpace(properties.lua)) lua = Warehouse.GetLua(properties.lua);
    }



    public void Draw(SpriteBatch spriteBatch, float tickProgress, Vector2 position)
    {
      // todo: handle spritesheets and multiple textures
      spriteBatch.Draw(texture, position - new Vector2(0, 1) * TILE_THICKNESS, Color.White);
    }


  }
}
