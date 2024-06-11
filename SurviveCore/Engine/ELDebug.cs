using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SurviveCore.Engine
{
  internal class ELDebug
  {
    public static bool Log(object output, bool error = false)
    {
      if (output == null) output = "<null>";
      Debug.WriteLine(output.ToString(), error? "[ERROR]" : "[log]");
      return false;
    }

  }
}
