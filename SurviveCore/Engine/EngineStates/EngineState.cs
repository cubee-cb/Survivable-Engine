using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.EngineStates
{
  public abstract class EngineState
  {
    protected Game1 game;
    protected GraphicsDeviceManager graphics;
    protected GraphicsDevice graphicsDevice;
    protected ContentManager content;
    protected GameWindow window;
    protected SpriteBatch spriteBatch;

    public EngineState(Game1 gameInstance)
    {
      game = gameInstance;
      graphicsDevice = game.GraphicsDevice;
      graphics = game._graphics;
      content = game.Content;
      window = game.Window;
      spriteBatch = game.spriteBatch;
    }

    public abstract void Init();
    public abstract void Update(float deltaTime);
    public abstract void Draw(float deltaTime);

  }
}
