using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public struct MobProperties
  {
    // descriptions
    public string textureSheetName = "mob_default";
    public string internalName = "mob_default";
    public List<string> tags = new();
    public List<string> sounds = new();

    // stats
    public int maxHealth = 10;
    public int inventorySize = 5;

    // lua
    public string lua;



    public MobProperties(string jsonObject)
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

      this = JsonConvert.DeserializeObject<MobProperties>(jsonObject);

    }

  }
}
