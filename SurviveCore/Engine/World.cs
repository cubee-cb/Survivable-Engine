using Microsoft.Xna.Framework.Graphics;
using SurviveCore.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  //todo: we could probably make a new class for each world type, but that's probably best left for the game content
  internal class World : Updatable, Renderable
  {
    List<WorldActor> actors;

    public World()
    {
      actors = new List<WorldActor>();
    }

    public void LoadGraphics(GraphicsDevice graphicsDevice)
    {
      foreach (WorldActor actor in actors)
      {
        actor.LoadGraphics(graphicsDevice);
      }
    }

    public void Update(int tick, float deltaTime)
    {

      // update world's actors
      //todo: create a partitioning system so only entities near the camera get updated
      foreach (WorldActor actor in actors)
      {
        actor.Update(tick, deltaTime);
      }

    }

    public void Draw(SpriteBatch spriteBatch)
    {
      // draw world's actors
      foreach (WorldActor actor in actors)
      {
        actor.Draw(spriteBatch);
      }


    }

    public void AddActor(WorldActor newActor)
    {
      actors.Add(newActor);
    }

    public void RemoveActor(WorldActor removeActor)
    {
      removeActor.Dispose();
      actors.Remove(removeActor);
    }



  }
}
