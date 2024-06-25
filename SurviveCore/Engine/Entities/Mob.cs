﻿using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using SurviveCore.Engine;
using SurviveCore.Engine.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Entities
{
  [MoonSharpUserData]
  internal class Mob : Entity
  {

    [JsonIgnore] MobProperties properties;

    // lua scripts
    [JsonIgnore] private Script lua;

    public Mob(string id, World world) : base(id, world)
    {

      // set initial properties
      properties = Warehouse.GetJson<MobProperties>(id);
      rotationType = properties.rotationType;
      spriteDimensions = properties.spriteDimensions;
      health = properties.maxHealth;
      tags = properties.tags;

      // load assets
      texture = Warehouse.GetTexture(properties.textureSheetName);
      if (properties.sounds != null)
      {
        foreach (string fileName in properties.sounds)
        {
          Warehouse.GetSoundEffect(fileName);
        }
      }

      // create inventory
      inventory = new(properties.inventorySize);

      // initialise lua
      if (!string.IsNullOrWhiteSpace(properties.lua))
      {
        lua = Warehouse.GetLua(properties.lua);

        // pass methods to lua
        lua.Globals["Move"] = (Func<float, float, float, bool>)Move;
        lua.Globals["MoveToward"] = (Func<float, float, float, bool>)MoveToward;
        lua.Globals["GetTarget"] = (Func<string, string, Table>)GetTarget;
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

    public override float GetStrength()
    {
      //todo: return attack power
      return properties.maxHealth;
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
      Vector2 moveVector = new(x - position.X, y - position.Y);
      moveVector.Normalize();

      if (Single.IsNaN(moveVector.X) || Single.IsNaN(moveVector.Y))
      {
        ELDebug.Log("tried to move toward same position --> normalised vector became NaN.");
        return false;
      }

      float movedDistance = TryMove(moveVector * speed).Length();

      return movedDistance >= moveVector.Length();
    }

    /// <summary>
    /// Get a target matching certain tags. If multiple are found, a random one is chosen other than the current target.
    /// </summary>
    /// <param name="tags">A list of tags to match.</param>
    /// <returns>The position of the target</returns>
    private Table GetTarget(string tag, string condition)
    {
      MatchCondition matchCondition = (MatchCondition)Enum.Parse(typeof(MatchCondition), condition);

      Entity foundEntity = world.FindEntityWithTag(this, tag, matchCondition);

      Vector2 target = GetPosition();
      if (foundEntity != null)
      {
        target = foundEntity.GetPosition();
      }

      // build table
      Table tbl = new(lua);
      tbl.Set("x", DynValue.NewNumber(target.X));
      tbl.Set("y", DynValue.NewNumber(target.Y));
      tbl.Set("valid", DynValue.NewBoolean(foundEntity != null));

      return tbl;

    }



  }
}