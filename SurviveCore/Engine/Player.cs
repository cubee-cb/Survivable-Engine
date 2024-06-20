using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using SurviveCore.Engine.Input;
using SurviveCore.Engine.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  internal class Player : Entity
  {

    [JsonIgnore] CharacterProperties properties;

    [JsonIgnore] private InputManager input;
    List<int> inventory;

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

      /*/ run mob's ai and tick scripts each tick
      if (lua != null)
      {
        DynValue resAI = lua.Call(lua.Globals["AI"]);

        DynValue resTick = lua.Call(lua.Globals["Tick"]);

      }
      //*/

    }


  }
}

