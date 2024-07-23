using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SurviveCore.Engine
{
  internal class ELDebug
  {
    /// <summary>
    /// Log a string to the console. Automatically calls ToString() on the input object.
    /// </summary>
    /// <param name="output">Object to print out.</param>
    /// <param name="error">Whether this log string should count as an error, or simply log info.</param>
    [Conditional("DEBUG")] // may revert this, and have a bool to toggle it for logging purposes later.
    public static void Log(object output, Category category = Category.Log)
    {
      if (output == null) output = "<null>";
      Debug.WriteLine(output.ToString(), '[' + category.ToString() + ']');
    }

    /// <summary>
    /// Quick function to get basic keyboard input in debug builds.
    /// </summary>
    /// <param name="key">The key to ckeck.</param>
    /// <returns>Whether the key is pressed.</returns>
    public static bool Key(Keys key)
    {
      return Keyboard.GetState().IsKeyDown(key);
    }

    /// <summary>
    /// Log categories for debug logging.
    /// </summary>
    public enum Category
    {
      Log,
      ERROR,
      Warning
    }


  }
}
