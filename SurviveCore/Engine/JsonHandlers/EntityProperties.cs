using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public readonly struct EntityProperties
  {
    // descriptions
    public readonly string textureSheetName = "entity.default";
    public readonly string internalName = "entity.default";
    public readonly List<string> tags = new();

    // stats
    public readonly int maxHealth = 10;

    // lua
    public readonly string luaTick;
    public readonly string luaInteract;
    public readonly string luaDamaged;
    public readonly string luaDestroyed;



    public EntityProperties(string jsonObject)
    {
      /*
      textureSheetName =;
      internalName = key;
      tags = new List<string>();

      maxHealth = 10;
      inventorySize = 5;

      luaAI = "";
      luaTick = "";
      luaInteract = "";
      luaDamaged = "";
      luaDefeated = "";
      */

      this = JsonConvert.DeserializeObject<EntityProperties>(jsonObject);

    }

  }
}
