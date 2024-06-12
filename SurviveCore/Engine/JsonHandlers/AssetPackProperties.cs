using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public struct AssetPackProperties
  {
    // properties
    public string internalName = "pack_default";
    public string author = "default";
    public string version = "1.0";
    public List<string> tags = new();

    public string nameSpace = "default";

    public AssetPackProperties(string jsonObject)
    {
      this = JsonConvert.DeserializeObject<AssetPackProperties>(jsonObject);
    }
  }
}
