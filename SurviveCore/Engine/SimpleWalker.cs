using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  internal class SimpleWalker : WorldActor
  {
    public SimpleWalker() : base()
    {
    }

    public override void LoadGraphics()
    {
      
    }

    public override void Update(int tick, float deltaTime)
    {
      velocity.X = 1;

      TryMove(velocity);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {

    }



  }
}
