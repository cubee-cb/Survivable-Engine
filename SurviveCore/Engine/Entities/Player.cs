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

    [JsonIgnore] CharacterProperties properties;

    [JsonIgnore] private InputManager input;

    // lua scripts
    //[JsonIgnore] private Script lua;

    public Player(string id, InputManager input, World world) : base(id, world)
    {
      this.input = input;

      // set initial properties
      properties = Warehouse.GetJson<CharacterProperties>(id);
      health = properties.maxHealth;

      // load assets
      texture = Warehouse.GetTexture(properties.textureSheetName);
      properties.spriteDimensions.TryGetValue("width", out spriteRect.Width);
      properties.spriteDimensions.TryGetValue("height", out spriteRect.Height);
      if (properties.sounds != null)
      {
        foreach (string fileName in properties.sounds)
        {
          Warehouse.GetSoundEffect(fileName);
        }
      }

      // create inventory
      inventory = new(properties.inventorySize);



      /*/ initialise lua
      if (!string.IsNullOrWhiteSpace(properties.lua))
      {
        lua = Warehouse.GetLua(properties.lua);

        // pass methods to lua
        lua.Globals["Move"] = (Func<float, float, float, bool>)Move;
        lua.Globals["MoveToward"] = (Func<float, float, float, bool>)MoveToward;
        lua.Globals["GetTarget"] = (Func<Table, Table>)GetTarget;
      }
      //*/

    }

    public override void Update(int tick, float deltaTime)
    {
      base.Update(tick, deltaTime);

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

      TryMove(velocity);

      /*/ run ai and tick scripts each tick
      if (lua != null)
      {
        DynValue resAI = lua.Call(lua.Globals["AI"]);

        DynValue resTick = lua.Call(lua.Globals["Tick"]);

      }
      //*/

      // set the sprite corresponding to the facing direction of the player
      Point a = new(0, properties.animationLayout.IndexOf(direction.ToString()));
      spriteRect.Location = a;
      ELDebug.Log(a + " / " + direction);
    }


  }
}

