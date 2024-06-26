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
          map.Plot(ix, iy, new GroundTile("test.grass", Game1.rnd.Next(0, 2)));
        }
      }



    }

  }
}
