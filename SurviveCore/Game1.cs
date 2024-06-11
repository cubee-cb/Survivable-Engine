using Microsoft.Xna.Framework;
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

    Texture2D texMissing;

    List<GameInstance> gameInstances;


    public Game1()
    {
      _graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      IsMouseVisible = true;
    }



    protected override void Initialize()
    {
      // TODO: Add your initialization logic here


      // initialise game instances (todo: these should only be done once player count and single/multiplayer has been chosen)
      gameInstances = new()
      {
        //new GameInstance(EInstanceMode.Server, -1, targetTickRate: 60, graphicsDevice: GraphicsDevice, Content),
        new GameInstance(EInstanceMode.Client, 0, targetTickRate: 10 /* 60 */, graphicsDevice: GraphicsDevice, Content)
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
      spriteBatch.Begin();

      foreach (GameInstance instance in gameInstances)
      {
        instance.Draw(spriteBatch, deltaTime);
      }

      spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
