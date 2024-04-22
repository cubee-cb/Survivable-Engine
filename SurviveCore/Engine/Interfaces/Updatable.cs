using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Interfaces
{
  internal interface Updatable
  {
    void Update(int tick, float deltaTime);

  }
}
