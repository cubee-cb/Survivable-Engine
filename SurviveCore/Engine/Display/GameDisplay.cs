using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SurviveCore.Engine.Display
{
  internal class GameDisplay
  {
    private static GameDisplay currentDisplayInstance;
    private static EGameDisplayLayer currentDisplayLayer;

    const int INTERNAL_WIDTH = 240;
    const int INTERNAL_HEIGHT = 200;

    int width;
    int height;
    float scaleFactor;

    float UIScaleMultiplier = 1.0f;
    
    private RenderTarget2D display;
    protected RenderTarget2D renderGameWorld;
    protected RenderTarget2D renderUI;
    protected RenderTarget2D renderOverlay;

    private static GraphicsDevice graphicsDevice;
    private static SpriteBatch spriteBatch;

    public GameDisplay(GraphicsDevice outerGraphicsDevice, int width, int height)
    {
      // set reference to the GraphicsDevice and SpriteBatch
      graphicsDevice = outerGraphicsDevice;
      spriteBatch = new SpriteBatch(outerGraphicsDevice);

      ScaleDisplay(width, height);
    }

    public float ScaleDisplay(int width, int height)
    {
      // clear displays
      display.Dispose();
      renderGameWorld.Dispose();
      renderUI.Dispose();
      renderOverlay.Dispose();

      this.width = width;
      this.height = height;

      float scale = Math.Min(width / INTERNAL_WIDTH, height / INTERNAL_HEIGHT);


      display = new RenderTarget2D(graphicsDevice, width, height);
      renderGameWorld = new RenderTarget2D(graphicsDevice, width, height);
      renderUI = new RenderTarget2D(graphicsDevice, width, height);
      renderOverlay = new RenderTarget2D(graphicsDevice, width, height);


      scaleFactor = scale;
      return scale;
    }

    public void Begin()
    {
      spriteBatch.Begin();
    }

    public void End()
    {
      spriteBatch.End();
    }

    public static void SetDisplayInstance(GameDisplay display)
    {
      currentDisplayInstance = display;
    }

    public void SetDisplayLayer(EGameDisplayLayer layer)
    {
      switch (layer)
      {
        case EGameDisplayLayer.Game:
          graphicsDevice.SetRenderTarget(renderGameWorld);
          break;
      }
    }

    public static void Draw(Texture2D texture, Rectangle clippingArea, Vector2 location, Color? colour = null, bool flipX = false, bool flipY = false, float angleTurns = 0)
    {
      if (colour == null) colour = Color.White;

      float depth = 0f;
      float scale = 1f;
      SpriteEffects effects = SpriteEffects.None;
      if (flipX) effects = SpriteEffects.FlipHorizontally;
      if (flipY) effects = SpriteEffects.FlipVertically; // todo: how to combine these? are these even what we want?


      //currentDisplayInstance
      spriteBatch.Draw(texture, location, clippingArea, (Color)colour, angleTurns, Vector2.One / 2f, scale, effects, depth);
    }

  }

  public enum EGameDisplayLayer
  {
    Game = 0,
    UI = 1,
    Overlay = 2,
  }
}
