using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public class GameProperties
  {
    // descriptions
    public string internalName = "test_game";
    public List<string> tags = new();

    // stats
    public string startingDimension = "dimension_default";


    // lua
    //public string lua;



    public GameProperties()
    {
    }

    public GameProperties(GameProperties source)
    {
      // set the following to source's fields if they aren't null, otherside back to themselves
      internalName = source.internalName ?? internalName;
      tags = source.tags ?? tags;
      startingDimension = source.startingDimension ?? startingDimension;
    }

  }
}
