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
      rotationType = properties.rotationType;
      spriteDimensions = properties.spriteDimensions;
      health = properties.maxHealth;
      tags = properties.tags;

      // manually add a tag that says this object is a player
      tags.Add("player");

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

      /*/ set the sprite corresponding to the facing direction of the player
      string dirString = GetRotationSpriteName();
      //if (dirString == "Left" && properties.animationLayout)
      Point a = new(0, rotationTypeToLayout[rotationType].IndexOf(dirString));
      spriteRect.Location = a;
      ELDebug.Log(a + " / " + direction);
      //*/
    }

    public override float GetStrength()
    {
      //todo: return attack power
      return properties.maxHealth;
    }


  }
}

