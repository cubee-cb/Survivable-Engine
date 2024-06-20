using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public struct ItemProperties
  {
    // descriptions
    public string textureSheetName = "item_default";
    public string internalName = "item_default";
    public List<string> tags = new();
    public List<string> sounds = new();

    // stats
    public SwingType swingType = SwingType.slash;
    public HoldType holdType = HoldType.none;


    public List<ItemComponent> components = new();

    // lua
    public string lua;



    public ItemProperties(string jsonObject)
    {
      this = JsonConvert.DeserializeObject<ItemProperties>(jsonObject);
    }


    public enum SwingType
    {
      slash = 0,
      poke = 1,
      hold = 2,
      consume = 3,
      mallet = 4
    }

    public enum HoldType
    {
      none = 0,
      inFront = 1,
      shoulder = 2
    }



  }
}
