using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SurviveCore.Engine.Entities;
using SurviveCore.Engine.WorldGen;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.WorldGen
{
  public class TileMap
  {
    public const int TILE_WIDTH = 16;
    public const int TILE_HEIGHT = 16;

    readonly public int width;
    readonly public int height;
    readonly private GroundTile[,] map;

    public TileMap(int width, int height)
    {
      this.width = width;
      this.height = height;

      map = new GroundTile[width, height];

    }
    public void UpdateAssets()
    {
      foreach (GroundTile tile in map)
      {
        tile.UpdateAssets();
      }
    }

    public bool Plot(int x, int y, GroundTile tile)
    {
      if (x < 0 || x >= width || y < 0 || y >= height)
      {
        return false;
      }

      map[x, y] = tile;

      return true;
    }

    public bool Plot(Vector2 position, GroundTile tile)
    {
      return Plot((int)position.X / TILE_WIDTH, (int)position.Y / TILE_HEIGHT, tile);
    }

    public bool SetElevation(int x, int y, int elevation)
    {
      if (x < 0 || x >= width || y < 0 || y >= height)
      {
        return false;
      }

      map[x, y].SetElevation(elevation);

      return true;
    }

    public bool SetElevation(Vector2 position, int elevation)
    {
      return SetElevation((int)position.X / TILE_WIDTH, (int)position.Y / TILE_HEIGHT, elevation);
    }

    /// <summary>
    /// Gets a Tile at tile coordinate. If pixel is set, uses pixel coordinates.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="pixel">Use pixel coordinates instead of tile coordinates.</param>
    /// <returns>The tile at the location.</returns>
    public GroundTile Get(int x, int y, bool pixel = false)
    {
      if (pixel)
      {
        x /= TILE_WIDTH;
        y /= TILE_HEIGHT;
      }

      if (x < 0 || x >= width || y < 0 || y >= height)
      {
        return null;
      }

      return map[x, y];
    }

    /// <summary>
    /// Gets a Tile at pixel coordinate.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public GroundTile Get(Vector2 position)
    {
      return Get((int)position.X, (int)position.Y, pixel: true);
    }

    public void Draw(float tickProgress)
    {
      for (int ix = 0; ix < width; ix++)
      {
        for (int iy = 0; iy < height; iy++)
        {
          GroundTile tile = map[ix, iy];
          if (tile != null) tile.Draw(tickProgress, new Vector2(ix * TILE_WIDTH, iy * TILE_HEIGHT - TILE_HEIGHT));
        }
      }

    }

    public static Vector2 SnapPosition(Vector2 position)
    {
      position.X = MathF.Floor(position.X / TILE_WIDTH) * TILE_WIDTH;
      position.Y = MathF.Floor(position.Y / TILE_HEIGHT) * TILE_HEIGHT;
      return position;
    }



  }
}
