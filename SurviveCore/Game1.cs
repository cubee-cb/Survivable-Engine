using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SurviveCore.Engine;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace SurviveCore
{
  public class Game1 : Game
  {
    public const string ENGINE_NAME = "Survivable Engine";

    readonly private GraphicsDeviceManager _graphics;
    private SpriteBatch spriteBatch;

    /* 
      change this to force a specific game.
      otherwise Warehouse will load the first game it finds, which may not be desired.
     */
    public const string GAME_NAMESPACE = "";

    public static Random rnd = new();
    private static GameWindow window;
    public static void SetWindowTitle(string windowTitle = "")
    {
      window.Title = windowTitle;
    }

    List<GameInstance> gameInstances;

    public Game1()
    {
      _graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      IsMouseVisible = true;

      Window.AllowUserResizing = true;
      window = Window;

      // uncomment these to uncap the framerate
      _graphics.SynchronizeWithVerticalRetrace = false;
      IsFixedTimeStep = false;
    }



    protected override void Initialize()
    {
      // TODO: Add your initialization logic here

      SetWindowTitle(ENGINE_NAME);
      AudioManager.Initialise();

      base.Initialize();
    }



    protected override void LoadContent()
    {
      spriteBatch = new SpriteBatch(GraphicsDevice);

      // start up warehouse and load asset packs
      Warehouse.SetGraphicsDevice(GraphicsDevice);
      Warehouse.LoadPlaceholders(Content);
      Warehouse.LoadGame(GAME_NAMESPACE);
      Warehouse.LoadAssetPacks();

      // initialise game instances (todo: these should only be done once player count and single/multiplayer has been chosen)
      gameInstances =
      [
        new GameInstance(EInstanceMode.Host, PlayerIndex.One, targetTickRate: 60, graphicsDevice: GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
        //new GameInstance(EInstanceMode.Client, PlayerIndex.Two, targetTickRate: 60, graphicsDevice: GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
        //new GameInstance(EInstanceMode.Client, PlayerIndex.Three, targetTickRate: 60, graphicsDevice: GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
        //new GameInstance(EInstanceMode.Client, PlayerIndex.Four, targetTickRate: 60, graphicsDevice: GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
      ];

    }



    protected override void Update(GameTime gameTime)
    {
      float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      // clean up finished sound effect instances
      AudioManager.Cleanup();

      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

      // update game instances
      foreach (GameInstance instance in gameInstances)
      {
        instance.Update(deltaTime);
      }

      base.Update(gameTime);
    }



    protected override void Draw(GameTime gameTime)
    {
      float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      GraphicsDevice.Clear(Color.CornflowerBlue);

      // dynamically figure out a somewhat reasonable grid size for the display, to fit the splitscreen displays
      // kind of sketchy, probably best to have some predefined layouts, then get generative for more extreme counts.
      int displayGridX = (int)Math.Ceiling(Math.Sqrt(gameInstances.Count));
      int displayGridY = (int)Math.Floor(Math.Sqrt(gameInstances.Count) + 0.5f);
      int displayIndex = 0;

      int displayWidth = Window.ClientBounds.Width / displayGridX;
      int displayHeight = Window.ClientBounds.Height / displayGridY;

      // draw game instances to their own textures
      List<Texture2D> displays = [];
      foreach (GameInstance instance in gameInstances)
      {
        // resize the displays to fit the grid layout
        instance.display.ScaleDisplay(displayWidth, displayHeight);

        // store rendered displays to actually draw later, so we can use one spritebatch for that instead of having to start and end one for each display
        displays.Add(instance.Draw(deltaTime));
        displayIndex++;
      }

      // draw game instances to the main display
      GraphicsDevice.SetRenderTarget(null);

      displayIndex = 0;
      spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
      foreach (Texture2D display in displays)
      {
        // figure out where this display should go based on the grid
        Rectangle bounds = new(new Point(displayIndex % displayGridX * displayWidth, (int)Math.Floor((double)displayIndex / displayGridX) * displayHeight), display.Bounds.Size);

        // draw it!
        spriteBatch.Draw(display, bounds, Color.White);

        displayIndex++;
      }

      spriteBatch.End();


      base.Draw(gameTime);
    }
  }
}
