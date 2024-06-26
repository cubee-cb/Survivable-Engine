using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  public enum MatchCondition
  {
    Random,
    Nearest,
    Farthest,
    MostDurable, // highest health
    MostFragile, // lowest health
    Strongest, // most attack power
    Weakest // least attack power
  }
}
