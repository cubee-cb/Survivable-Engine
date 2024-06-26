using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Entities
{
  /// <summary>
  /// Possible directions an entity can face.
  /// </summary>
  public enum FacingDirection
  {
    Unchanged = -1,
    Right = 0,
    RightDown = 1,
    Down = 2,
    LeftDown = 3,
    Left = 4,
    LeftUp = 5,
    Up = 6,
    RightUp = 7
  }
}
