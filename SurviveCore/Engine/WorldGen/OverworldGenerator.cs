using SurviveCore.Engine.WorldGen.Routines;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.WorldGen
{
  internal class OverworldGenerator : WorldGenerator
  {
    public OverworldGenerator() : base()
    {
      AddRoutine(new GM_FlatGrass());
    }


  }
}
