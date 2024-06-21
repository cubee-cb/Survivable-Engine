using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Entities
{
  /// <summary>
  /// Determines how the sprites should be set up for rotations.
  /// </summary>
  public enum SpriteRotationType
  {
    None, // forward only
    TwoWay, // right, left
    Cardinal, // up, down, right, left
    Diagonal, // up-right, down-right, up-left, down-left
    EightWay, // up, up-right, right, down-right, down, down-left, left, up-left
  }
}
