using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.Interfaces
{
  internal interface Renderable
  {
    void LoadGraphics(GraphicsDevice graphicsDevice);

    void Draw(SpriteBatch spriteBatch);


  }
}
