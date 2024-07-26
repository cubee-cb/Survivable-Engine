using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SurviveCore.Engine.Display
{
  public class GameDisplay
  {
    private static GameDisplay currentDisplayInstance;
    private static EGameDisplayLayer currentDisplayLayer;

    const int BASE_INTERNAL_WIDTH = 128;
    const int BASE_INTERNAL_HEIGHT = 128;
    public const int OVERDRAW_MARGIN = 1;

    public int width;
    public int height;
    public int internalWidth;
    public int internalHeight;
    float scaleFactor;

    private Vector2 cameraPosition;

    float UIScaleMultiplier = 1.0f;
    
    private RenderTarget2D display;
    protected RenderTarget2D renderGameWorld;
    protected RenderTarget2D renderUI;
    protected RenderTarget2D renderOverlay;

    private static GraphicsDevice graphicsDevice;
    private SpriteBatch spriteBatch;

    //private static Font activeFont;

    public GameDisplay(GraphicsDevice outerGraphicsDevice, int width, int height)
    {
      // set reference to the GraphicsDevice and SpriteBatch
      graphicsDevice = outerGraphicsDevice;
      spriteBatch = new SpriteBatch(outerGraphicsDevice);
      cameraPosition = Vector2.Zero;

      ScaleDisplay(width, height);
    }

    public Vector2 Camera()
    {
      return cameraPosition;
    }

    public Vector2 Camera(Vector2 newPos)
    {
      cameraPosition = newPos;
      return cameraPosition;
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
      Rectangle pixelRect = new Rectangle(internalOffsetX, internalOffsetY, targetInternalWidth, targetInternalHeight);
      spriteBatch.Draw(renderGameWorld, pixelRect, Color.White);

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

    /// <summary>
    /// Draw a texture to the currently active display instance.
    /// </summary>
    /// <param name="texture">Texture to draw with.</param>
    /// <param name="clippingArea">Clipping rectangle for the texture.</param>
    /// <param name="location">Position to display the sprite at.</param>
    /// <param name="colour">Colour to render with.</param>
    /// <param name="flipX">Flip horizontally.</param>
    /// <param name="flipY">Flip vertically.</param>
    /// <param name="angleTurns">Angle from 0-1.</param>
    /// <param name="visualOffsetY">Visual offset. Doesn't affect depth.</param>
    /// <param name="depth">Depth to render at.</param>
    public static void Draw(Texture2D texture, Rectangle clippingArea, Vector2 location, Color? colour = null, bool flipX = false, bool flipY = false, float angleTurns = 0, float visualOffsetX = 0, float visualOffsetY = 0, float depth = -1, Point? scaleBox = null)
    {
      if (texture == null)
      {
        ELDebug.Log("a null texture was passed to Draw.", ELDebug.Category.Warning);
        return;
      }
      if (colour == null) colour = Color.White;
      location -= currentDisplayInstance.cameraPosition;
      //location = Vector2.Floor(location);

      if (depth == -1) depth = (1 - (location.Y / currentDisplayInstance.internalHeight)) / 2f + 0.25f;
      SpriteEffects effects = SpriteEffects.None;
      if (flipX) effects = SpriteEffects.FlipHorizontally;
      if (flipY) effects = SpriteEffects.FlipVertically; // todo: how to combine these? are these even what we want?

      //todo: angleTurns is still radians instead of turns. FIX IT
      Vector2 position = Vector2.Floor(location + new Vector2(visualOffsetX, visualOffsetY));
      Point destinationSize = clippingArea.Size;
      if (scaleBox != null) destinationSize *= (Point)scaleBox;
      currentDisplayInstance.spriteBatch.Draw(texture, new Rectangle(position.ToPoint(), destinationSize), clippingArea, (Color)colour, angleTurns, Vector2.One / 2f, effects, depth);
    }
    /*/
    public static void setActiveFont(Font font)
    {
      activeFont = font;
    }
    //*/

    /// <summary>
    /// Draw some text to the screen using a Font, according to the provided options.
    /// </summary>
    /// <param name="input">The object to print. Calls ToString() on the input.</param>
    /// <param name="position">Locaiton to start printing from.</param>
    /// <param name="font">The Font object to use.</param>
    /// <param name="colour">Colour to tint the font with.</param>
    /// <param name="variableWidth">If true (default), use the glyph size overrides provided by the font. If false, use the base glyph size for all characters.</param>
    /// <returns>A bounding rectangle for the displayed text.</returns>
    public static Rectangle Print(object input, Vector2 position, Font font, Color? colour = null, bool variableWidth = true)
    {
      if (colour == null) colour = Color.White;
      position -= currentDisplayInstance.cameraPosition;
      position = Vector2.Floor(position);

      int scribeX = 0;
      foreach (char c in input.ToString())
      {
        int charIndex = font.glyphOrder.IndexOf(c);
        if (charIndex == -1) ELDebug.Log("character '" + c + "' not found in font.");
        int thisCharWidth = font.glyphSize;
        if (variableWidth && font.glyphSizeOverrides != null && font.glyphSizeOverrides.ContainsKey(c)) thisCharWidth = font.glyphSizeOverrides[c];

        Rectangle clippingArea = new(charIndex * (font.glyphSize + font.glyphGap), 0, thisCharWidth, font.texture.Height);

        currentDisplayInstance.spriteBatch.Draw(font.texture, position + new Vector2(scribeX + 0.5f, 0.5f), clippingArea, (Color)colour, 0, Vector2.One / 2f, Vector2.One, SpriteEffects.None, 0);

        scribeX += thisCharWidth + font.glyphGap;
      }

      return new((int)position.X, (int)position.Y, scribeX, font.texture.Height);
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
