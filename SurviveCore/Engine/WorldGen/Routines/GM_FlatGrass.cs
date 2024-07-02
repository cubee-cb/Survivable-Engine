using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.WorldGen.Routines
{
  internal class GM_FlatGrass : WorldGenRoutine
  {
    public override void Run(TileMap map)
    {
      ELDebug.Log("running gm_flatgrass routine");

      // create grass
      for (int ix = 0; ix < map.width; ix++)
      {
        for (int iy = 0; iy < map.height; iy++)
        {
          string tile = "test.grass";
          int elevation = 0;
          if (ix == 4 && iy > 3) tile = "test.grass_slope_horizontal";
          if (iy == 3 && ix < 4) tile = "test.grass_slope_vertical";
          if (ix > 4 || iy < 3) elevation = 1;
          map.Plot(ix, iy, new GroundTile(tile, elevation));
        }
      }



    }

  }
}
