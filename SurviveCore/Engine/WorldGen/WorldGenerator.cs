using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.WorldGen
{
  public class WorldGenerator
  {
    private readonly Dictionary<string, Script> worldgenRoutines = new();
    private TileMap activeMap;

    /// <summary>
    /// Create a generator with one routine.
    /// </summary>
    /// <param name="scriptID"></param>
    public WorldGenerator(string scriptID)
    {
      AddRoutine(scriptID);
    }

    /// <summary>
    /// Create a generator with multiple routines.
    /// </summary>
    /// <param name="scriptIDs"></param>
    public WorldGenerator(List<string> scriptIDs)
    {
      foreach (string scriptID in scriptIDs)
      {
        AddRoutine(scriptID);
      }
    }

    /// <summary>
    /// Add a routine to this generator.
    /// </summary>
    /// <param name="scriptID"></param>
    public void AddRoutine(string scriptID)
    {
      Script routine = Warehouse.GetLua(scriptID);

      // register methods to the script
      routine.Globals["Plot"] = (Func<int, int, string, bool>)Plot;
      routine.Globals["SetElevation"] = (Func<int, int, int, bool>)SetElevation;

      //todo: create a conversion for TileMap<->Array and for TileEntities
      worldgenRoutines.Add(scriptID, routine);
    }

    /// <summary>
    /// Run this generator's routines on a map.
    /// </summary>
    /// <param name="map">The map for the routines to modify.</param>
    public void Generate(TileMap map)
    {
      ELDebug.Log("generating world...");
      activeMap = map;

      // run routines
      foreach (KeyValuePair<string, Script> kvp in worldgenRoutines)
      {
        Script routine = kvp.Value;
        if (routine == null)
        {
          ELDebug.Log("routine \"" + kvp.Key + "\" is null");
        }
        else
        {
          ELDebug.Log("running routine \"" + kvp.Key + "\"");

          DynValue generate = routine.Globals.Get("Generate");
          if (generate != null)
          {
            DynValue outVal = routine.Call(generate, 0, 0, map.width, map.height);
          }
        }
      }

      activeMap = null;
      ELDebug.Log("finished generation!");
    }


    //                           //
    // Lua-exposed methods below //
    // v v v v v v v v v v v v v //


    public bool Plot(int x, int y, string tileID)
    {
      //todo: i forgor
      return activeMap.Plot(new Vector2(x * TileMap.TILE_WIDTH, y * TileMap.TILE_HEIGHT), new GroundTile(tileID, 0));
    }

    public bool SetElevation(int x, int y, int elevation)
    {
      return activeMap.SetElevation(new Vector2(x * TileMap.TILE_WIDTH, y * TileMap.TILE_HEIGHT), elevation);
    }



  }
}
