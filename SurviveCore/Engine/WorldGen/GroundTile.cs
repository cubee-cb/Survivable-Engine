using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using SurviveCore.Engine.Display;
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

      // set initial properties
      properties = Warehouse.GetJson<GroundProperties>(id);
      this.elevation = elevation;

      // load assets
      texture = Warehouse.GetTexture(properties.textureSheetName);
      if (properties.sounds != null)
      {
        foreach (string fileName in properties.sounds)
        {
          Warehouse.GetSoundEffect(fileName);
        }
      }

      // initialise lua
      if (!string.IsNullOrWhiteSpace(properties.lua)) lua = Warehouse.GetLua(properties.lua);
    }

    /// <summary>
    /// Return the elevation of this tile's top
    /// </summary>
    /// <param name="pixels">If true, return the elevation in pixels rather than the tile's thickness units.</param>
    /// <returns></returns>
    public int GetElevation(bool pixels = false)
    {
      return pixels? elevation * TILE_THICKNESS : elevation;
    }


    public void Draw(float tickProgress, Vector2 position)
    {
      // todo: handle spritesheets and multiple textures
      GameDisplay.Draw(texture, new Rectangle(0, 0, 16, 32), position, visualOffsetY: -(elevation * TILE_THICKNESS - 16), colour: elevation == 0 ? Color.LightGray : Color.White);
    }


  }
}
