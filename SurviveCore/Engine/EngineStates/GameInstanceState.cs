using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurviveCore.Engine.EngineStates
{
  public class GameInstanceState : EngineState
  {


    List<GameInstance> gameInstances;

    public GameInstanceState(Game1 gameInstance) : base(gameInstance)
    {
    }

    public override void Init()
    {
      // initialise game instances (todo: these should only be done once player count and single/multiplayer has been chosen)
      gameInstances = new()
      {
        new GameInstance(EInstanceMode.Host, PlayerIndex.One, targetTickRate: 30, graphicsDevice: graphicsDevice, content, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
        //new GameInstance(EInstanceMode.Client, PlayerIndex.Two, targetTickRate: 30, graphicsDevice: graphicsDevice, content, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
        //new GameInstance(EInstanceMode.Client, PlayerIndex.Three, targetTickRate: 30, graphicsDevice: graphicsDevice, content, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
        //new GameInstance(EInstanceMode.Client, PlayerIndex.Four, targetTickRate: 30, graphicsDevice: graphicsDevice, content, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
      };

    }

    public override void Update(float deltaTime)
    {
      // update game instances
      foreach (GameInstance instance in gameInstances)
      {
        instance.Update(deltaTime);
      }

    }

    public override void Draw(float deltaTime)
    {
      // dynamically figure out a somewhat reasonable grid size for the display, to fit the splitscreen displays
      // kind of sketchy, probably best to have some predefined layouts, then get generative for more extreme counts.
      int displayGridX = (int)Math.Ceiling(Math.Sqrt(gameInstances.Count));
      int displayGridY = (int)Math.Floor(Math.Sqrt(gameInstances.Count) + 0.5f);
      int displayIndex = 0;

      int displayWidth = window.ClientBounds.Width / displayGridX;
      int displayHeight = window.ClientBounds.Height / displayGridY;

      // draw game instances to their own textures
      List<Texture2D> displays = new();
      foreach (GameInstance instance in gameInstances)
      {
        // resize the displays to fit the grid layout
        instance.display.ScaleDisplay(displayWidth, displayHeight);

        // store rendered displays to actually draw later, so we can use one spritebatch for that instead of having to start and end one for each display
        displays.Add(instance.Draw(deltaTime));
        displayIndex++;
      }

      // draw game instances to the main display
      graphicsDevice.SetRenderTarget(null);

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

    }
  }
}
