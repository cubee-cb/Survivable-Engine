using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using SurviveCore.Engine.Display;
using System;
using System.Collections.Generic;
using System.Text;
using SurviveCore.Engine.JsonHandlers;
using static SurviveCore.Engine.JsonHandlers.GroundProperties;

namespace SurviveCore.Engine.WorldGen
{
  internal class GroundTile
  {
    private string id;
    private const int TILE_THICKNESS = 16;

    private int elevation = 0;

    [JsonIgnore] GroundProperties properties;
    [JsonIgnore] private Texture2D texture;
    [JsonIgnore] private Script lua;

    public GroundTile(string id, int elevation)
    {
      this.id = id;
      this.elevation = elevation;

      UpdateAssets();
    }

    public void UpdateAssets()
    {
      // set initial properties
      properties = Warehouse.GetJson<GroundProperties>(id);

      // load assets
      texture = Warehouse.GetTexture(properties.textureSheetName);

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

    public SlopeType GetSlope()
    {
      return properties.slope;
    }


    public void Draw(float tickProgress, Vector2 position)
    {
      // todo: handle spritesheets and multiple textures
      GameDisplay.Draw(texture, new Rectangle(0, 32, 16, 16), position, visualOffsetY: -(elevation * TILE_THICKNESS) + 32, colour: elevation == 0 ? Color.LightGray : Color.White, scaleBox: new(1, elevation + 1));

      // top face
      GameDisplay.Draw(texture, new Rectangle(0, 0, 16, 32), position, visualOffsetY: -(elevation * TILE_THICKNESS), colour: elevation == 0 ? Color.LightGray : Color.White);
    }


  }
}
