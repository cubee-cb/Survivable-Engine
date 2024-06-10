using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SurviveCore.Engine.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  internal class GameInstance
  {
    private int tickRate;
    private int tick;
    private float deltaTimeAccumulated;

    GraphicsDevice graphicsDevice;
    Warehouse warehouse;
    Texture2D missingTex;

    private List<World> worlds;
    private int activeWorldIndex = 0;
    World activeWorld;

    public GameInstance(int targetTickRate, GraphicsDevice graphicsDevice, ContentManager contentManager)
    {
      // initialise warehouse
      warehouse = new Warehouse(contentManager);
      Warehouse.SetNameSpace("everlost");

      tickRate = targetTickRate;
      tick = 0;
      deltaTimeAccumulated = 0;

      worlds = new List<World>();
      activeWorldIndex = 0;

      //todo: temp; need to figure out how world storage is going to work, and load from file/generate worlds as needed
      World tempWorld = new World(this);
      //tempWorld.AddActor(new SimpleWalker());

      // create a test mob
      Mob testMob = new Mob("mob.testghost", tempWorld);
      tempWorld.AddEntity(testMob);

      worlds.Add(tempWorld);
      this.graphicsDevice = graphicsDevice;

      LoadContent();

      ELDebug.Log("game instance initialised");
    }

    public void LoadContent()
    {

    }

    public void Update(float deltaTime)
    {
      activeWorld = worlds[activeWorldIndex];

      // run ticks to fill the time we've accumulated
      //todo: should we run ticks per-object?
      // just leave it global per-world?
      // move it to be per instance?
      // what about the player's updating?
      // how do we handle smoothing between low tickrates?
      deltaTimeAccumulated += deltaTime;

      // this lets up catch up if we fall behind (assuming the device can handle it, otherwise this is will progressively take more performance), or slow down if we're going too fast.
      // doesn't matter too much if we use fixed time step as the default is, but if we want to disable it, it shouldn't affect too much?
      float targetDeltaTime = 1f / tickRate;
      while (deltaTimeAccumulated > targetDeltaTime)
      {
        activeWorld.Update(tick, deltaTime);

        tick++;
        deltaTimeAccumulated -= targetDeltaTime; // is it correct to use targetDeltaTime? or will we overshoot or something?
        ELDebug.Log("ping! acc delta: " + deltaTimeAccumulated + " / " + targetDeltaTime + " (took " + deltaTime + " this frame)");
      }

    }

    public void Draw(SpriteBatch spriteBatch, float deltaTime)
    {


      // pass tick progress as 1, we have no smoothing yet
      activeWorld.Draw(spriteBatch, tickProgress: 1);
    }

  }
}
