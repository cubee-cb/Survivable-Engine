using SurviveCore.Engine.WorldGen.Routines;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.WorldGen
{
  internal abstract class WorldGenerator
  {
    readonly List<WorldGenRoutine> routines;

    protected WorldGenerator()
    {
      routines = new List<WorldGenRoutine>();
    }

    public void AddRoutine(WorldGenRoutine routine)
    {
      routines.Add(routine);
    }

    public virtual void Generate(TileMap map)
    {
      ELDebug.Log("generating world...");
      foreach (WorldGenRoutine routine in routines)
      {
        routine.Run(map);
      }

      ELDebug.Log("done!");
    }

  }
}
