using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore
{
  internal interface Locomotable
  {

    Vector2 TryMove(Vector2 delta);

  }
}
