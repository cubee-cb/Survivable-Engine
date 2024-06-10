using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public readonly struct EntityProperties
  {
    // descriptions
    public readonly string textureSheetName;
    public readonly string internalName;
    public readonly List<string> tags;

    // stats
    public readonly int maxHealth;
    public readonly int inventorySize;

    // lua
    public readonly string luaAI;
    public readonly string luaTick;
    public readonly string luaInteract;
    public readonly string luaDamaged;
    public readonly string luaDefeated;



    public EntityProperties(string jsonObject)
    {
      textureSheetName = "defaultEntityTexture";
      internalName = "default entity";
      tags = new List<string>();

      maxHealth = 10;
      inventorySize = 5;

      luaAI = "";
      luaTick = "";
      luaInteract = "";
      luaDamaged = "";
      luaDefeated = "";


    }

  }
}
