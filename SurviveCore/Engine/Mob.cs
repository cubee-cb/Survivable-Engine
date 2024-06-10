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
    [JsonIgnore] World world;

    [JsonIgnore] MobProperties properties;

    List<int> inventory;

    // lua scripts
    [JsonIgnore] private Script lua;

    public Mob(string id, World world) : base(id)
    {
      this.world = world;


      properties = Warehouse.GetJson<MobProperties>(id);

      texture = Warehouse.GetTexture(properties.textureSheetName);
      health = properties.maxHealth;

      // create inventory
      inventory = new List<int>();
      for (int i = 0; i < properties.inventorySize; i++)
      {
        inventory.Add(0);
      }

      // initialise lua
      lua = Warehouse.GetLua(properties.lua);
      lua.Globals["Move"] = (Func<float, float, bool>)Move;
      lua.Globals["MoveToward"] = (Func<float, float, bool>)MoveToward;
      lua.Globals["GetTarget"] = (Func<Table, Table>)GetTarget;

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


    private bool Move(float x, float y)
    {
      position.X += x;
      position.Y += y;
      return true;
    }

    private bool MoveToward(float x, float y)
    {
      position.X += Math.Sign(position.X - x);
      position.Y += Math.Sign(position.Y - y);
      return true;
    }

    private Table GetTarget(Table tags)
    {
      // find target matching tags
      foreach (string tag in tags.Values.AsObjects())
      {
        ELDebug.Log(tag);
      }

      return new Table(lua, DynValue.NewNumber(0), DynValue.NewNumber(0));

    }



  }
}
