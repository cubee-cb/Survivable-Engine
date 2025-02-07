using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using SurviveCore.Engine.Input;
using SurviveCore.Engine.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Entities
{
  internal class Player : Entity
  {
    [JsonIgnore] readonly private InputManager input;

    public Player(string id, InputManager input, World world) : base(id, world)
    {
      this.input = input;

      UpdateAssets();
    }

    public override void UpdateAssets()
    {

      // set initial properties
      properties = Warehouse.GetJson<CharacterProperties>(id);
      base.UpdateAssets();


      // manually add a tag that says this object is a player
      tags.Add("player");


      /*/ initialise lua
      if (!string.IsNullOrWhiteSpace(properties.lua))
      {
        // pass methods to lua
        lua.Globals["Move"] = (Func<float, float, float, bool>)Move;
      }
      //*/
    }

    public override void Update(int tick, float deltaTime)
    {
      base.PreUpdate(tick, deltaTime);

      // movmement
      velocity = Vector2.Zero;
      float speed = input.Action("run") ? properties.movementSpeedRun : properties.movementSpeedWalk;
      if (input.Action("left"))
      {
        velocity.X = -speed;
      }
      if (input.Action("right"))
      {
        velocity.X = speed;
      }
      if (input.Action("up"))
      {
        velocity.Y = -speed;
      }
      if (input.Action("down"))
      {
        velocity.Y = speed;
      }


      //todo: placeholder jump action
      if (grounded && input.Action("use"))
      {
        velocityElevation = 4.5f;
      }

      TryMove(velocity);

      /*/ run update lua
      if (lua != null)
      {
        DynValue resUpdate = lua.Call(lua.Globals["Update"]);
      }
      //*/

      base.PostUpdate(tick, deltaTime);
    }

    public override float GetStrength()
    {
      //todo: return attack power
      return properties.maxHealth;
    }


  }
}

