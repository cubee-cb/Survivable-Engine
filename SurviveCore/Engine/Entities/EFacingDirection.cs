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
    Right = 0,
    RightDown = 1,
    DownRight = 1,
    Down = 2,
    DownLeft = 3,
    LeftDown = 3,
    Left = 4,
    LeftUp = 5,
    UpLeft = 5,
    Up = 6,
    UpRight = 7,
    RightUp = 7
  }
}
