using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public struct ModProperties
  {
    public string internalName = "default_mod";
    public string nameSpace = "default_mod";
    public int revision = 0;

    public ModProperties(string jsonObject)
    {
      this = JsonConvert.DeserializeObject<ModProperties>(jsonObject);
    }

    public override readonly string ToString()
    {
      return internalName + " (rev" + revision + ") as \"" + nameSpace + "\"";
    }

  }
}
