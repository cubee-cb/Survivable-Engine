using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public class WorldProperties
  {

    public string internalName = "entity_default";
    public Dictionary<string, int> area;
    public List<string> tags = new();

    public List<string> generationRoutines = new();

    // lua
    public string lua;

    public WorldProperties()
    {
    }

    /// <summary>
    /// Update the properties for this object.
    /// </summary>
    /// <param name="source"></param>
    public virtual void ReplaceData(WorldProperties source)
    {
      // set the following to source's fields if they aren't null, otherwise back to themselves

      // descriptions
      internalName = source.internalName ?? internalName;
      area = source.area ?? area;
      tags = source.tags ?? tags;

      generationRoutines = source.generationRoutines ?? generationRoutines;

      // lua
      lua = source.lua ?? lua;

    }
  }
}

