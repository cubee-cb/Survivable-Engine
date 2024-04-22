using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SurviveCore.Engine;
using SurviveCore.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore
{
  internal abstract class WorldActor : IdentifiableObject, Locomotable, Renderable, Updatable
  {
    protected Vector2 position;
    protected Vector2 velocity;
    public Texture2D texture;

    public WorldActor() : base()
    {
    }

    public virtual void LoadGraphics(GraphicsDevice graphicsDevice)
    {
      texture = new Texture2D(graphicsDevice, 1, 1);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
      throw new NotImplementedException();
    }

    public virtual void Update(int tick, float deltaTime)
    {
      throw new NotImplementedException();
    }

    public virtual Vector2 TryMove(Vector2 delta)
    {
      // todo: check for collisions with objects this object is allowed to collide with

      position += delta;
      return Vector2.Zero;
    }

    public void Dispose()
    {
      texture.Dispose();
    }


  }
}
