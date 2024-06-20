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
    None,
    TwoWayFlip, // right
    TwoWay, // right, left
    CardinalFlip, // up, down, right
    Cardinal, // up, down, right, left
    DiagonalFlip, // up-right, down-right
    Diagonal, // up-right, down-right, up-left, down-left
    EightWayFlip, // up, up-right, right, down-right, down
    EightWay, // up, up-right, right, down-right, down, down-left, left, up-left
  }
}
