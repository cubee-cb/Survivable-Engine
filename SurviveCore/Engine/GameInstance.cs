﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SurviveCore.Engine.JsonHandlers;
using SurviveCore.Engine.WorldGen;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  internal class GameInstance
  {
    EInstanceMode instanceMode;
    int playerIndex;

    private int tickRate;
    private int tick;
    private float deltaTimeAccumulated;

    GraphicsDevice graphicsDevice;
    Warehouse warehouse;
    Texture2D missingTex;

    private List<World> worlds;
    private int activeWorldIndex = 0;
    World activeWorld;

    public GameInstance(EInstanceMode instanceMode, int playerIndex, int targetTickRate, GraphicsDevice graphicsDevice, ContentManager contentManager)
    {
      this.instanceMode = instanceMode;
      this.playerIndex = playerIndex;

      // initialise warehouse
      warehouse = new Warehouse(contentManager, graphicsDevice);
      Warehouse.SetNameSpace("everlost");

      tickRate = targetTickRate;
      tick = 0;
      deltaTimeAccumulated = 0;

      worlds = new List<World>();
      activeWorldIndex = 0;

      //todo: temp; need to figure out how world storage is going to work, and load from file/server/generate worlds as needed
      World tempWorld = new(this, 10, 10, new OverworldGenerator());

      //tempWorld.AddActor(new SimpleWalker());

      // create local player (unless this is a dedicated server)
      if (instanceMode != EInstanceMode.Dedicated)
      {
        //tempWorld.AddEntity(new Player(playerIndex, tempWorld));
      }

      // create a test mob
      Mob testMob = new("mob_testghost", tempWorld);
      tempWorld.AddEntity(testMob);

      worlds.Add(tempWorld);
      this.graphicsDevice = graphicsDevice;

      ELDebug.Log("game instance initialised");
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

        ELDebug.Log("ping! total delta: " + deltaTimeAccumulated + "ms > " + targetDeltaTime + "ms (took " + deltaTime + "ms this real frame)");

        tick++;
        deltaTimeAccumulated -= targetDeltaTime; // is it correct to use targetDeltaTime? or will we overshoot or something?
      }

    }

    public void Draw(SpriteBatch spriteBatch, float deltaTime)
    {
      // dedicated servers don't need to bother with rendering
      if (instanceMode != EInstanceMode.Dedicated)
      {
        // pass tick progress to draw, so objects can visually smooth to their new location
        activeWorld.Draw(spriteBatch, deltaTimeAccumulated * tickRate); // eqiv. to (deltaTimeAcc / targetDeltaTime)
      }
    }

  }
}
