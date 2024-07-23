using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SurviveCore.Engine.Display;
using SurviveCore.Engine.Entities;
using SurviveCore.Engine.Input;
using SurviveCore.Engine.Items;
using SurviveCore.Engine.JsonHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine
{
  internal class GameInstance
  {
    EInstanceMode instanceMode;
    PlayerIndex playerIndex;

    private int targetTickRate;
    private int tickRate;
    private int tick;
    private float deltaTimeAccumulated;

    GraphicsDevice graphicsDevice;
    public GameDisplay display;
    InputManager input;

    Player player;
    List<Entity> cameraFocusEntities;

    private List<World> worlds;
    private int activeWorldIndex = 0;
    World activeWorld;

    GameProperties gameProps;

    public GameInstance(EInstanceMode instanceMode, PlayerIndex playerIndex, int targetTickRate, GraphicsDevice graphicsDevice, ContentManager contentManager, int displayWidth, int displayHeight)
    {
      this.instanceMode = instanceMode;
      this.playerIndex = playerIndex;

      // initialise display and input
      display = new GameDisplay(graphicsDevice, displayWidth, displayHeight);
      input = new InputManager(playerIndex, hasKeyboard: playerIndex == PlayerIndex.One);

      gameProps = Warehouse.GetGameProps();

      this.targetTickRate = targetTickRate;
      tickRate = targetTickRate;
      tick = 0;
      deltaTimeAccumulated = 0;

      worlds = new List<World>();
      activeWorldIndex = 0;

      //todo: temp; need to figure out how world storage is going to work, and load from file/server/generate worlds as needed
      World tempWorld = new(gameProps.startingDimension, this);

      // create local player (unless this is a dedicated server)
      if (instanceMode != EInstanceMode.Dedicated)
      {
        player = new(gameProps.startingPlayer, input, tempWorld);
        tempWorld.AddEntity(player);
      }

      // create test mobs
      foreach (string mobID in gameProps.startingMobs)
      {
        tempWorld.AddEntity(new Mob(mobID, tempWorld));
      }

      // tell camera to focus on this entity
      cameraFocusEntities = new()
      {
        player
      };

      foreach (string itemID in gameProps.startingInventory)
      {
        //todo: make this entry use actual ItemsProperties in json?
        // so games can start players off with modified and custom items, say a damaged axe or something
        player.GetInventory().AddItem(new Item(itemID));
      }

      worlds.Add(tempWorld);
      this.graphicsDevice = graphicsDevice;

      ELDebug.Log("game instance initialised");
    }

    public void Update(float deltaTime)
    {
      // tickrate modifier keys
      tickRate = targetTickRate;
      if (ELDebug.Key(Keys.J)) tickRate /= 8;
      if (ELDebug.Key(Keys.K)) tickRate *= 8;
      if (ELDebug.Key(Keys.L)) tickRate *= 16;

      // dangerous keys (hold right control to activate)
      if (ELDebug.Key(Keys.RightControl))
      {
        // unload all asset packs ([U]nload)
        if (ELDebug.Key(Keys.U))
        {
          Warehouse.UnloadAll();
          UpdateAssets();
        }
        // load all asset packs ([I]nitialise)
        if (ELDebug.Key(Keys.I))
        {
          Warehouse.LoadAll();
          UpdateAssets();
        }
      }



      activeWorld = worlds[activeWorldIndex];

      // buffer input
      input.BufferInputs();

      // run ticks to fill the time we've accumulated
      deltaTimeAccumulated += deltaTime;

      // this lets us catch up if we fall behind, or slow down if we're going too fast.
      //todo: may break if the device is running too slowly, we'll see
      float targetDeltaTime = 1f / tickRate;
      while (deltaTimeAccumulated > targetDeltaTime)
      {
        activeWorld.Update(tick, deltaTime);

        if (ELDebug.Key(Keys.LeftAlt)) ELDebug.Log("ping! (" + tickRate + " TPS) total delta: " + deltaTimeAccumulated + "s > " + targetDeltaTime + "s (took " + deltaTime + "s this real frame)");

        tick++;
        deltaTimeAccumulated -= targetDeltaTime;

        input.ResetInputs();
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
        // move camera to follow targeted entities
        //todo: average position of all targeted entities. never let index 0 go off-screen
        if (cameraFocusEntities.Count > 0)
        {
          Entity followEntity = cameraFocusEntities[0];
          display.Camera(followEntity.GetVisualPosition(tickProgress) - Vector2.UnitY * followEntity.GetVisualElevation(tickProgress) - new Vector2(display.internalWidth, display.internalHeight) / 2);
        }

        // pass tick progress to draw, so objects can visually smooth to their new location
        activeWorld.Draw(tickProgress); // eqiv. to (deltaTimeAcc / targetDeltaTime)

        display.Camera(Vector2.Zero);

        display.End();


        // draw to the UI layer
        display.SetDisplayLayer(EGameDisplayLayer.UI);
        display.Begin();
        display.Camera(Vector2.Zero);

        // draw player's inventory
        player.GetInventory().Draw(Vector2.Zero, 100, tickProgress);

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

    private void UpdateAssets()
    {
      foreach (World world in worlds)
      {
        world.UpdateAssets();
      }
    }

    public int GetTickRate()
    {
      return tickRate;
    }


  }
}
