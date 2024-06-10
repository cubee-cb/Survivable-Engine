using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SurviveCore.Engine
{
  internal class ELDebug
  {
    public static void Log(object output, bool error = false)
    {
      Debug.WriteLine(output.ToString(), error? "[ERROR]" : "[log]");
    }

  }
}
