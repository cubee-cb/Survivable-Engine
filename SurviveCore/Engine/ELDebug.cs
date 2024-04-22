using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SurviveCore.Engine
{
  internal class ELDebug
  {
    public static void Log(object output)
    {
      Debug.WriteLine("[log] " + output.ToString());
    }

  }
}
