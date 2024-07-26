using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SurviveCore.Engine;
using SurviveCore.Engine.Display;
using SurviveCore.Engine.EngineStates;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace SurviveCore
{
  public class Game1 : Game
  {
    public GraphicsDeviceManager _graphics;
    public GameWindow _window;
    public SpriteBatch spriteBatch;
    public GameDisplay engineDisplay;

    public static Random rnd = new Random();

    private EngineState engineState;
    private EngineState queuedEngineState;

    public Game1()
    {
      _graphics = new GraphicsDeviceManager(this);
      _window = Window;
      Content.RootDirectory = "Content";
      IsMouseVisible = true;

      Window.AllowUserResizing = true;

      // uncomment these to uncap the framerate
      _graphics.SynchronizeWithVerticalRetrace = false;
      IsFixedTimeStep = false;

      // change this for other games
      //Warehouse.SetGameFolder("surviveEngine");
    }



    protected override void Initialize()
    {
      // TODO: Add your initialization logic here

      // initialise the engien display and use it.
      engineDisplay = new(GraphicsDevice, Window.ClientBounds.Width, Window.ClientBounds.Width);
      GameDisplay.SetDisplayInstance(engineDisplay);

      base.Initialize();
    }



    protected override void LoadContent()
    {
      spriteBatch = new SpriteBatch(GraphicsDevice);

      Warehouse.SetGraphicsDevice(GraphicsDevice);
      // load fallback content for warehouse, used when an asset cannot be found
      // content.Load is only used here for built-in engine content like placeholders.
      Warehouse.missingTexture = Content.Load<Texture2D>("spr/missing");
      Warehouse.missingSound = Content.Load<SoundEffect>("sfx/missing");
      Warehouse.missingMusic = Content.Load<Song>("music/missing");


      QueueEngineState(new LoadAssetsState(this));
    }



    protected override void Update(GameTime gameTime)
    {
      float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      // change to the queued engine state
      if (queuedEngineState != null)
      {
        engineState = queuedEngineState;
        queuedEngineState = null;

        engineState.Init();
      }

      // clean up finished sound effect instances
      AudioManager.Cleanup();

      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

      engineState.Update(deltaTime);

      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
      float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      GraphicsDevice.Clear(Color.CornflowerBlue);

      engineState.Draw(deltaTime);


      base.Draw(gameTime);
    }

    /// <summary>
    /// Changes the engine state out for a new state, and runs its Init method.
    /// </summary>
    /// <param name="state">The state instance to change to.</param>
    public void QueueEngineState(EngineState state)
    {
      queuedEngineState = state;
    }
  }
}
