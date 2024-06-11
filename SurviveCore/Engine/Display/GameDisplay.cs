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

    const int BASE_INTERNAL_WIDTH = 128;
    const int BASE_INTERNAL_HEIGHT = 128;
    public const int OVERDRAW_MARGIN = 1;

    int width;
    int height;
    int internalWidth;
    int internalHeight;
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

    public float ScaleDisplay(int _width, int _height)
    {
      // dispose the displays
      display?.Dispose();
      renderGameWorld?.Dispose();
      renderUI?.Dispose();
      renderOverlay?.Dispose();

      width = _width;
      height = _height;

      scaleFactor = (float)Math.Floor((double)Math.Min(width / BASE_INTERNAL_WIDTH, height / BASE_INTERNAL_HEIGHT));

      // OVERDRAW_MARGIN is multiplied by 2 so it's added to both sides
      internalWidth = (int)(width / scaleFactor) + OVERDRAW_MARGIN * 2;
      internalHeight = (int)(height / scaleFactor) + OVERDRAW_MARGIN * 2;

      // create the displays
      display = new RenderTarget2D(graphicsDevice, width, height);
      renderGameWorld = new RenderTarget2D(graphicsDevice, internalWidth, internalHeight);
      renderUI = new RenderTarget2D(graphicsDevice, width, height);
      renderOverlay = new RenderTarget2D(graphicsDevice, width, height);

      return scaleFactor;
    }

    public void Begin()
    {
      spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
    }

    public void End()
    {
      spriteBatch.End();
    }

    public void SetDisplayLayer(EGameDisplayLayer layer)
    {
      switch (layer)
      {
        case EGameDisplayLayer.Base:
          graphicsDevice.SetRenderTarget(display);
          graphicsDevice.Clear(Color.Black);
          break;
        case EGameDisplayLayer.Game:
          graphicsDevice.SetRenderTarget(renderGameWorld);
          graphicsDevice.Clear(Color.CornflowerBlue);
          break;
        case EGameDisplayLayer.UI:
          graphicsDevice.SetRenderTarget(renderUI);
          graphicsDevice.Clear(Color.Transparent);
          break;
        case EGameDisplayLayer.Overlay:
          graphicsDevice.SetRenderTarget(renderOverlay);
          graphicsDevice.Clear(Color.Transparent);
          break;
      }
    }

    public Texture2D ComposeLayers()
    {
      graphicsDevice.SetRenderTarget(display);
      graphicsDevice.Clear(Color.Black);
      spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

      // make sure all the layers scale nicely
      int targetInternalWidth = (int)(internalWidth * scaleFactor);
      int targetInternalHeight = (int)(internalHeight * scaleFactor);
      int internalOffsetX = (width - targetInternalWidth) / 2;
      int internalOffsetY = (height - targetInternalHeight) / 2;

      // draw the layers
      spriteBatch.Draw(renderGameWorld, new Rectangle(internalOffsetX, internalOffsetY, targetInternalWidth, targetInternalHeight), Color.White);

      spriteBatch.Draw(renderUI, display.Bounds, Color.White);
      spriteBatch.Draw(renderOverlay, display.Bounds, Color.White);

      spriteBatch.End();

      // return the display as a texture, to use later
      return display;
    }




    // statics

    public static void SetDisplayInstance(GameDisplay display)
    {
      currentDisplayInstance = display;
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
    Base = 0,
    Game = 1,
    UI = 2,
    Overlay = 3,
  }
}
