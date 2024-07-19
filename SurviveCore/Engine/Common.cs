using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  public static class Common
  {
    /// <summary>
    /// Linear Interpolation between two values by position.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <param name="position">0-1 value. 0 is fully first and 1 is fully second.</param>
    /// <returns></returns>
    /*/
    public static float Lerp(float first, float second, float position)
    {
      position = Clamp(position, 0, 1);
      return first * (1 - position) + second * position;
    }
    //*/

    /// <summary>
    /// Clamps a value to the range between min and max.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float Clamp(float value, float min, float max)
    {
      return MathF.Max(min, MathF.Min(value, max));
    }

  }
}
