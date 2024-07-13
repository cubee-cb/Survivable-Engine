using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.JsonHandlers
{
  public class MobProperties : EntityProperties
  {


    public MobProperties() : base()
    {
    }

    /// <summary>
    /// Update the properties for this object. Use the base class overload to update only the base class's fields.
    /// </summary>
    /// <param name="source"></param>
    public void ReplaceData(MobProperties source)
    {
      base.ReplaceData(source);
    }

  }
}
