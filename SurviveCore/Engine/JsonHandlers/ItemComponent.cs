using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public struct ItemComponent
  {
    public string type = "default_component";
    public int damage = 0;
    public int heals = 0;



    public ItemComponent(string jsonObject)
    {
      this = JsonConvert.DeserializeObject<ItemComponent>(jsonObject);
    }



  }
}
