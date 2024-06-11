using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public struct EntityProperties
  {
    // descriptions
    public string textureSheetName = "entity_default";
    public string internalName = "entity_default";
    public List<string> tags = new();
    public List<string> sounds = new();

    // stats
    public int maxHealth = 10;

    // lua
    public string lua;



    public EntityProperties(string jsonObject)
    {
      /*
      textureSheetName =;
      internalName = key;
      tags = new List<string>();

      maxHealth = 10;
      inventorySize = 5;

      lua = "";
      */

      this = JsonConvert.DeserializeObject<EntityProperties>(jsonObject);

    }

  }
}
