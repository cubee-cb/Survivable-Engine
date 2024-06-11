﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SurviveCore.Engine;
using System;
using System.Collections.Generic;

namespace SurviveCore
{
  public class Game1 : Game
  {
    private GraphicsDeviceManager _graphics;
    private SpriteBatch spriteBatch;

    public static Random rnd = new Random();
    public static GraphicsDevice graphicsDevice;

    List<GameInstance> gameInstances;

    public Game1()
    {
      _graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      IsMouseVisible = true;

      Window.AllowUserResizing = true;


      // uncomment these to uncap the framerate
      _graphics.SynchronizeWithVerticalRetrace = false;
      IsFixedTimeStep = false;
    }



    protected override void Initialize()
    {
      // TODO: Add your initialization logic here


      // initialise game instances (todo: these should only be done once player count and single/multiplayer has been chosen)
      gameInstances = new()
      {
        new GameInstance(EInstanceMode.Host, 0, targetTickRate: 30, graphicsDevice: GraphicsDevice, Content, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
        //new GameInstance(EInstanceMode.Client, 1, targetTickRate: 30, graphicsDevice: GraphicsDevice, Content, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
        //new GameInstance(EInstanceMode.Client, 2, targetTickRate: 30, graphicsDevice: GraphicsDevice, Content, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
        //new GameInstance(EInstanceMode.Client, 3, targetTickRate: 30, graphicsDevice: GraphicsDevice, Content, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
      };

      base.Initialize();
    }



    protected override void LoadContent()
    {
      spriteBatch = new SpriteBatch(GraphicsDevice);
      graphicsDevice = GraphicsDevice;

      // TODO: use this.Content to load your game content here

    }



    protected override void Update(GameTime gameTime)
    {
      float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

      // TODO: Add your update logic here
      //ELDebug.Log("update!");
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

      // TODO: Add your drawing code here

      // draw game instances
      List<Texture2D> displays = new();
      int displayGridX = (int)Math.Ceiling(Math.Sqrt(gameInstances.Count));
      int displayGridY = (int)Math.Floor(Math.Sqrt(gameInstances.Count) + 0.5f);
      int displayIndex = 0;


      ELDebug.Log(displayGridX + " x " + displayGridY + " (" + 0 + "x)");

      int displayWidth = _graphics.PreferredBackBufferWidth / displayGridX;
      int displayHeight = _graphics.PreferredBackBufferHeight / displayGridY;
      foreach (GameInstance instance in gameInstances)
      {
        //todo: this should handle sizing the displays to fit the grid layout
        instance.display.ScaleDisplay(displayWidth, displayHeight);

        displays.Add(instance.Draw(deltaTime));
        displayIndex++;
      }

      // draw game instances to the main display
      graphicsDevice.SetRenderTarget(null);

      displayIndex = 0;
      spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
      foreach (Texture2D display in displays)
      {
        Rectangle bounds = new Rectangle(new Point(displayIndex % displayGridX * displayWidth, (int)Math.Floor((double)displayIndex / displayGridX) * displayHeight), display.Bounds.Size);

        spriteBatch.Draw(display, bounds, Color.White);

        displayIndex++;
      }

      spriteBatch.End();


      base.Draw(gameTime);
    }
  }
}
