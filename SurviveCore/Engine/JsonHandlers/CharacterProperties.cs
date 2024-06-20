using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public struct CharacterProperties
  {
    // descriptions
    public string textureSheetName = "player_default";
    public string internalName = "player_default";
    public List<string> tags = new();
    public List<string> sounds = new();

    // stats
    public int maxHealth = 10;
    public float regenerationPerSecond = 1;

    public float movementSpeedWalk = 1f;
    public float movementSpeedRun = 1.5f;

    public int inventorySize = 5;


    // lua
    //public string lua;



    public CharacterProperties(string jsonObject)
    {
      this = JsonConvert.DeserializeObject<CharacterProperties>(jsonObject);
    }

  }
}
