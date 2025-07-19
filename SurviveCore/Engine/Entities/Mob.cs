using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using SurviveCore.Engine;
using SurviveCore.Engine.JsonHandlers;
using SurviveCore.Engine.Lua;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Entities
{
  [MoonSharpUserData]
  internal class Mob : Entity
  {
    public Mob(string id, World world) : base(id, world)
    {
      UpdateAssets();
    }

    public override void UpdateAssets()
    {
      // set initial properties
      properties = Warehouse.GetJson<MobProperties>(id);
      base.UpdateAssets();
    }

    public override void Update(int tick, float deltaTime)
    {
      base.PreUpdate(tick, deltaTime);

      // run mob's ai and tick functions each tick
      if (lua != null)
      {
        DynValue resAI = LuaCommon.TryRunMethod(lua, "AI");
        DynValue resTick = LuaCommon.TryRunMethod(lua, "Tick");
      }

      base.PostUpdate(tick, deltaTime);
    }

    public override float GetStrength()
    {
      //todo: return attack power
      return properties.maxHealth;
    }

    public override void OnCollisionEnter(Entity otherEntity)
    {
      OnCollisionEnter();

      // run mob's collision enter function
      //todo: have separate OnCollision and OnDamaged functions
      if (lua != null) LuaCommon.TryRunMethod(lua, "CollisionEnter", otherEntity.GetTags());

    }

    public override void OnCollisionStay(Entity otherEntity)
    {
      OnCollisionStay();

      // run mob's collision enter function
      //todo: have separate OnCollision and OnDamaged functions
      if (lua != null) LuaCommon.TryRunMethod(lua, "CollisionStay", otherEntity.GetTags());

    }

    public override void OnCollisionExit(Entity otherEntity)
    {
      OnCollisionEnter();

      // run mob's collision exit function
      if (lua != null) LuaCommon.TryRunMethod(lua, "CollisionExit", otherEntity.GetTags());

    }


  }
}
