using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  internal interface IHasAssets
  {
    /// <summary>
    /// Called when this object should re-obtain its assets.
    /// </summary>
    /// <param name="id">The string id of the object. Handled on a per-subclass basis.</param>
    public void UpdateAssets();
  }
}
