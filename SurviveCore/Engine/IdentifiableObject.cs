using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  internal abstract class IdentifiableObject
  {
    private const int id;

    public IdentifiableObject()
    {
      id = GetHashCode();
    }

    int GetID()
    {
      return id;
    }

  }
}
