using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using SurviveCore.Engine.Display;
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

      //todo: maybe reimplement player code as lua?

      // movement
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
      if (grounded && input.Action("jump"))
      {
        velocityElevation = 4.5f;
      }

      TryMove(velocity);

      //todo: use held items
      if (input.Action("use"))
      {
        // player can spiiinnnn in place by pressing "use"
        Vector2 aimCursor = input.GetAimCursor();

        //todo: why doesn't this work? player just snaps to look right
        direction = GetFacingDirection(aimCursor - position);

        // yippee!
        UseHeldItem();
      }
      else
      {
        // might be a crutch; how do i check in Entity if the held item is no-longer being "use"d?
        ReleaseUseHeldItem();
      }

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




    public override void Draw(float tickProgress)
    {
      base.Draw(tickProgress);

      //todo: temporary draw aim cursor
      GameDisplay.Draw(shadowTexture, shadowTexture.Bounds, input.GetAimCursor(), visualOffsetX: -shadowTexture.Width / 2, visualOffsetY: -shadowTexture.Height / 2, colour: Color.White * 0.5f, layer: 9);

    }


  }
}

