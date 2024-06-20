using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SurviveCore.Engine.Display;
using SurviveCore.Engine.Input;
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
    PlayerIndex playerIndex;

    private int tickRate;
    private int tick;
    private float deltaTimeAccumulated;

    GraphicsDevice graphicsDevice;
    public GameDisplay display;
    InputManager input = new();
    Warehouse warehouse;
    Texture2D missingTex;

    Player player;
    List<Entity> cameraFocusEntities;

    private List<World> worlds;
    private int activeWorldIndex = 0;
    World activeWorld;

    public GameInstance(EInstanceMode instanceMode, PlayerIndex playerIndex, int targetTickRate, GraphicsDevice graphicsDevice, ContentManager contentManager, int displayWidth, int displayHeight)
    {
      this.instanceMode = instanceMode;
      this.playerIndex = playerIndex;

      // initialise display
      display = new GameDisplay(graphicsDevice, displayWidth, displayHeight);

      // initialise warehouse
      warehouse = new Warehouse(contentManager, graphicsDevice);
      Warehouse.SetNameSpace("test");

      tickRate = targetTickRate;
      tick = 0;
      deltaTimeAccumulated = 0;

      worlds = new List<World>();
      activeWorldIndex = 0;

      //todo: temp; need to figure out how world storage is going to work, and load from file/server/generate worlds as needed
      World tempWorld = new(10, 10, new OverworldGenerator());

      //tempWorld.AddActor(new SimpleWalker());

      // create local player (unless this is a dedicated server)
      if (instanceMode != EInstanceMode.Dedicated)
      {
        player = new("character_test", input, tempWorld);
        tempWorld.SetPlayerRef(player);
      }

      // create a test mob
      Mob testMob = new("mob_testghost", tempWorld);
      tempWorld.AddEntity(testMob);

      // tell camera to focus on this entity
      cameraFocusEntities = new()
      {
        testMob
      };

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
      deltaTimeAccumulated += deltaTime;

      // this lets up catch up if we fall behind (assuming the device can handle it, otherwise this is will progressively take more performance), or slow down if we're going too fast.
      // doesn't matter too much if we use fixed time step as the default is, but if we want to disable it, it shouldn't affect too much?
      float targetDeltaTime = 1f / tickRate;
      while (deltaTimeAccumulated > targetDeltaTime)
      {
        input = new InputManager(playerIndex, hasKeyboard: playerIndex == PlayerIndex.One);


        player.Update(tick, deltaTime);
        activeWorld.Update(tick, deltaTime);

        ELDebug.Log("ping! total delta: " + deltaTimeAccumulated + "ms > " + targetDeltaTime + "ms (took " + deltaTime + "ms this real frame)");

        tick++;
        deltaTimeAccumulated -= targetDeltaTime; // is it correct to use targetDeltaTime? or will we overshoot or something?
      }

    }

    public Texture2D Draw(float deltaTime)
    {
      // dedicated servers don't need to bother with rendering
      if (instanceMode != EInstanceMode.Dedicated)
      {
        // calculate time between last and next scheduled tick, for smoothing
        float tickProgress = deltaTimeAccumulated * tickRate;

        // set GameDisplay methods to draw to this instance's screen
        GameDisplay.SetDisplayInstance(display);


        // draw to the game world layer
        display.SetDisplayLayer(EGameDisplayLayer.Game);
        display.Begin();
        // move camera to follow targete entities
        //todo: average position of all targeted entities. never let index 0 go off-screen
        display.Camera(cameraFocusEntities[0].GetVisualPosition(tickProgress) - new Vector2(display.internalWidth, display.internalHeight) / 2);

        // pass tick progress to draw, so objects can visually smooth to their new location
        activeWorld.Draw(tickProgress); // eqiv. to (deltaTimeAcc / targetDeltaTime)

        /*/ debug things
        for (int i = 0; i < 500; i++)
        {
          GameDisplay.Draw(Warehouse.GetTexture("everlost.mob_testghost"), new Rectangle(0, 0, 16, 16), new Vector2(i * 12, i));
        }
        //*/

        display.End();


        // draw to the UI layer
        display.SetDisplayLayer(EGameDisplayLayer.UI);
        display.Begin();
        display.Camera(Vector2.Zero);

        /*/ debug things
        for (int i = 0; i < 500; i++)
        {
          GameDisplay.Draw(Warehouse.GetTexture("everlost.mob_testghost"), new Rectangle(0, 0, 16, 16), new Vector2(i * 12, i));
        }
        //*/

        display.End();


        // draw to overlay layer
        display.SetDisplayLayer(EGameDisplayLayer.Overlay);
        display.Begin();
        display.Camera(Vector2.Zero);

        display.End();


        // compose display layers to its base layer, and return them to the main loop
        return display.ComposeLayers();

      }

      return null;
    }

  }
}
