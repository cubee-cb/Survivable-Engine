using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SurviveCore.Engine.WorldGen;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.WorldGen
{
  internal class TileMap
  {
    const int TILE_WIDTH = 16;
    const int TILE_HEIGHT = 16;

    readonly public int width;
    readonly public int height;
    readonly private GroundTile[,] map;

    public TileMap(int width, int height)
    {
      this.width = width;
      this.height = height;

      map = new GroundTile[width, height];

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

    public void Draw(SpriteBatch spriteBatch, float tickProgress)
    {
      for (int ix = 0; ix < width; ix++)
      {
        for (int iy = 0; iy < height; iy++)
        {
          GroundTile tile = map[ix, iy];
          tile.Draw(spriteBatch, tickProgress, new Vector2(ix * TILE_WIDTH, iy * TILE_HEIGHT));
        }
      }

    }



  }
}
