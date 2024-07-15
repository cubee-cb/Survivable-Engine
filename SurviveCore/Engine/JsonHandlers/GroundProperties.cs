using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public class GroundProperties
  {
    // descriptions
    public string textureSheetName = "ground_default";
    public string internalName = "ground_default";
    public List<string> tags = new();
    public List<string> sounds = new();

    // stats
    public SlopeType slope = SlopeType.None;
    //public int elevation = 0;
    //public int elevation = 0;

    // lua
    public string lua;



    public GroundProperties()
    {
    }

    public void ReplaceData(GroundProperties source)
    {
      // set the following to source's fields if they aren't null, otherwise back to themselves

      // descriptions
      textureSheetName = source.textureSheetName ?? textureSheetName;
      internalName = source.internalName ?? internalName;
      tags = source.tags ?? tags;

      // stats
      slope = source.slope;

      // lua
      lua = source.lua ?? lua;

    }


    public enum SlopeType
    {
      None = 0,
      Horizontal = 1,
      Vertical = 2,
    }

  }
}
