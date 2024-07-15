using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public class GameProperties
  {
    // descriptions
    public List<string> tags = new();

    // stats
    public string startingDimension = "dimension_default";
    public string startingPlayer = "character_default";
    public List<string> startingInventory = new();
    public List<string> startingMobs = new();


    // lua
    //public string lua;



    public GameProperties()
    {
    }

    public void ReplaceData(GameProperties source)
    {
      // set the following to source's fields if they aren't null, otherwise back to themselves
      tags = source.tags ?? tags;

      // stats
      startingDimension = source.startingDimension ?? startingDimension;
      startingPlayer = source.startingPlayer ?? startingPlayer;
      startingInventory = source.startingInventory ?? startingInventory;
      startingMobs = source.startingMobs ?? startingMobs;


    }

  }
}
