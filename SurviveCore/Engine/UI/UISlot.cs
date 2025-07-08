using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter;

namespace SurviveCore.Engine.UI
{

  public struct UISlot
  {
    // descriptions
    public string texture = "uislot_default";
    public string name = "slot";
    public bool locked = false; // use in json to disable interactions on a slot, e.g. for viewing another player's inventory
    public List<float> position = new();

    public UISlot()
    {
      
    }

  }
}
