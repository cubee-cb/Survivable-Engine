using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using SurviveCore.Engine.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  [MoonSharpUserData]
  internal class Mob : Entity
  {

    [JsonIgnore] MobProperties properties;

    List<int> inventory;

    // lua scripts
    [JsonIgnore] private Script lua;

    public Mob(string id, World world) : base(id, world)
    {

      // set initial properties
      properties = Warehouse.GetJson<MobProperties>(id);
      health = properties.maxHealth;

      // load assets
      texture = Warehouse.GetTexture(properties.textureSheetName);
      if (properties.sounds != null)
      {
        foreach (string fileName in properties.sounds)
        {
          Warehouse.GetSoundEffect(fileName);
        }
      }

      // create inventory (temoporary; will use Item class when implemented)
      inventory = new List<int>();
      for (int i = 0; i < properties.inventorySize; i++)
      {
        inventory.Add(0);
      }

      // initialise lua
      if (!string.IsNullOrWhiteSpace(properties.lua))
      {
        lua = Warehouse.GetLua(properties.lua);

        // pass methods to lua
        lua.Globals["Move"] = (Func<float, float, float, bool>)Move;
        lua.Globals["MoveToward"] = (Func<float, float, float, bool>)MoveToward;
        lua.Globals["GetTarget"] = (Func<Table, Table>)GetTarget;
      }

    }

    public override void Update(int tick, float deltaTime)
    {
      base.Update(tick, deltaTime);

      // run mob's ai and tick scripts each tick
      if (lua != null)
      {
        DynValue resAI = lua.Call(lua.Globals["AI"]);

        DynValue resTick = lua.Call(lua.Globals["Tick"]);

      }

    }

    //                           //
    // Lua-exposed methods below //
    // v v v v v v v v v v v v v //

    /// <summary>
    /// Move in a direction with speed. Vector is normalised.
    /// </summary>
    /// <param name="x">Vector X component.</param>
    /// <param name="y">Vector Y component.</param>
    /// <param name="speed">Distance to move.</param>
    /// <returns>Whether the move was successful or not.</returns>
    private bool Move(float x, float y, float speed)
    {
      Vector2 moveVector = new Vector2(x, y);
      moveVector.Normalize();

      float movedDistance = TryMove(moveVector * speed).Length();

      return movedDistance >= moveVector.Length();
    }

    /// <summary>
    /// Move toward an absolute position. with speed. Vector is normalised.
    /// </summary>
    /// <param name="x">X coordinate to move toward.</param>
    /// <param name="y">Y coordinate to move toward.</param>
    /// <param name="speed">Distance to move.</param>
    /// <returns>Whether the move was successful or not.</returns>
    private bool MoveToward(float x, float y, float speed)
    {
      Vector2 moveVector = new Vector2(Math.Sign(x - position.X), Math.Sign(y - position.Y));
      moveVector.Normalize();

      float movedDistance = TryMove(moveVector * speed).Length();

      return movedDistance >= moveVector.Length();
    }

    /// <summary>
    /// Get a target matching certain tags. If multiple are found, a random one is chosen other than the current target.
    /// </summary>
    /// <param name="tags">A list of tags to match.</param>
    /// <returns>The position of the target</returns>
    private Table GetTarget(Table tags)
    {
      /*/ todo:
       * add priority selection (closest, random, strongest, same if possible)
       * find targets
       * filter out current target
       * choose one
       * return its position
      //*/

      // find target matching tags
      foreach (string tag in tags.Values.AsObjects())
      {
        //ELDebug.Log(tag);
      }

      Vector2 target = new Vector2(Game1.rnd.Next(0, 512), Game1.rnd.Next(0, 512));

      return new Table(lua, DynValue.NewNumber(target.X), DynValue.NewNumber(target.Y));

    }



  }
}
