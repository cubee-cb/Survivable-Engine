using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.UI
{

  public struct UIWidget
  {
    // descriptions
    public string texture = "uiwidget_default";
    public string name = "widget";
    public int frame = 0;
    public List<float> position = new();

    public UIWidget()
    {
      
    }

  }
}
