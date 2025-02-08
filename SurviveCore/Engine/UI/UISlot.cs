using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.UI
{


  public abstract struct UISlot
  {
    // descriptions
    public string texture = "uislot_default";
    public string name = "slot";
    public List<float> position = new();


  }
}
