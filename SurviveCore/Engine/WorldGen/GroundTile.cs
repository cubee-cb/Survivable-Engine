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
  public class GroundTile
  {
    private string id;

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

    /// <summary>
    /// Re-obtain the assets for this tile.
    /// </summary>
    public void UpdateAssets()
    {
      // set initial properties
      properties = Warehouse.GetJson<GroundProperties>(id);

      // load assets
      texture = Warehouse.GetTexture(properties.texture);

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
      return pixels? elevation * TileMap.TILE_THICKNESS : elevation;
    }

    /// <summary>
    /// Set the elevation of this tile.
    /// </summary>
    /// <param name="newElevation">The elevation to change to.</param>
    public void SetElevation(int newElevation)
    {
      elevation = newElevation;
    }

    /// <summary>
    /// Get the slope of this tile.
    /// </summary>
    /// <returns>Slope value.</returns>
    public SlopeType GetSlope()
    {
      return properties.slope;
    }


    public void Draw(float tickProgress, Vector2 position)
    {
      //todo: handle spritesheets
      //todo: autotiling
      //todo: tile animations
      //todo: draw neighbour tiles beneath, to fill transparent "blending" areas in the sprites.

      Color myColour = elevation == 0 ? Color.LightGray : Color.White;

      // front face
      // draw a layer lower so it doesn't draw in front of slopes
      GameDisplay.Draw(texture, new Rectangle(0, 32, 16, 16), position, visualOffsetY: -(elevation * TileMap.TILE_THICKNESS) + 16, colour: myColour, layer: elevation - 1);

      // top face
      GameDisplay.Draw(texture, new Rectangle(0, 0, 16, 32), position, visualOffsetY: -(elevation * TileMap.TILE_THICKNESS) - 16, colour: myColour, layer: elevation);
    }


  }
}
