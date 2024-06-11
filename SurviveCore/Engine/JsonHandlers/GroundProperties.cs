using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public struct GroundProperties
  {
    // descriptions
    public string textureSheetName = "ground_default";
    public string internalName = "ground_default";
    public List<string> tags = new();
    public List<string> sounds = new();

    // stats
    //public int elevation = 0;
    //public int elevation = 0;

    // lua
    public string lua;



    public GroundProperties(string jsonObject)
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

      this = JsonConvert.DeserializeObject<GroundProperties>(jsonObject);

    }

  }
}
