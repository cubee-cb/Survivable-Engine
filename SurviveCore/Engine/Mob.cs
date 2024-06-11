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
      if (properties.textureSheetName != null) texture = Warehouse.GetTexture(properties.textureSheetName);
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
        lua.Globals["DebugLog"] = (Func<object, bool, bool>)ELDebug.Log;
        lua.Globals["Move"] = (Func<float, float, bool>)Move;
        lua.Globals["MoveToward"] = (Func<float, float, bool>)MoveToward;
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


    private bool Move(float x, float y)
    {
      TryMove(new Vector2(x, y));
      return true;
    }

    private bool MoveToward(float x, float y)
    {
      TryMove(new Vector2(Math.Sign(x - position.X), Math.Sign(y - position.Y)));
      return true;
    }

    private Table GetTarget(Table tags)
    {
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
