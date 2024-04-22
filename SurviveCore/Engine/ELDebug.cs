using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  internal class ELDebug
  {
    public static void Log(object output)
    {
      Console.WriteLine("[log] " + output.ToString());
    }

  }
}
