using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  internal class SimpleWalker : WorldActor
  {
    /*
    public override void LoadGraphics(GraphicsDevice graphicsDevice)
    {
      
    }
    */

    public override void Update(int tick, float deltaTime)
    {
      velocity.X = 1;

      TryMove(velocity);
    }



  }
}
