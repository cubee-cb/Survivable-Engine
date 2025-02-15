﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  internal abstract class IdentifiableObject
  {
    private readonly int uid;

    public IdentifiableObject()
    {
      uid = GetHashCode();
    }

    public int GetUID()
    {
      return uid;
    }

  }
}
