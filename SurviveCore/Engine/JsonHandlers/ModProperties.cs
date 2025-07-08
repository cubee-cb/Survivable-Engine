using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public class ModProperties
  {
    public string internalName = "default_mod";
    public string nameSpace = "default_mod";
    public List<string> authors = new() { "author" };
    public int revision = 0;

    public ModProperties()
    {
    }

    public virtual void ReplaceData(ModProperties source)
    {
      // set the following to source's fields if they aren't null, otherwise back to themselves

      internalName = source.internalName ?? internalName;
      nameSpace = source.nameSpace ?? nameSpace;
      authors = source.authors ?? authors;

      revision = source.revision;
    }

    public override string ToString()
    {
      return internalName + " (rev" + revision + ") as \"" + nameSpace + "\"";
    }

  }
}
