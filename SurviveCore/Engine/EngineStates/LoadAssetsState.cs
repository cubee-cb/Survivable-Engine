using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SurviveCore.Engine.Display;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace SurviveCore.Engine.EngineStates
{
  public class LoadAssetsState : EngineState
  {
    Font font;
    bool loading;
    Thread loadingThread;


    public LoadAssetsState(Game1 gameInstance) : base(gameInstance)
    {
      font = new Font();
      font.texture = content.Load<Texture2D>("spr/system_font");
      font.glyphGap = 1;
      font.glyphSize = 7;
      font.glyphOrder = " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890():;<>.,\"'!?[]-$*_+=&#^\\|{}@`~%/";

    }
    public override void Init()
    {
      // make sure warehouse is empty
      //Warehouse.UnloadAll();

      // create a thread to start up warehouse and load asset packs
      loadingThread = new(() => { Warehouse.LoadAll(); });
      loading = false;

    }

    public override void Update(float deltaTime)
    {
      if (!loading)
      {
        // start thread
        loadingThread.Start();

        loading = true;
      }

      // change state once assets are loaded
      if (loading && loadingThread.ThreadState == ThreadState.Stopped)
      {
        game.QueueEngineState(new GameInstanceState(game));
      }
    }

    public override void Draw(float deltaTime)
    {
      // display loading screen
      // draw game instances to the main display
      graphicsDevice.SetRenderTarget(null);

      //todo: temp: shouldn't use game layer for drawing UIs, but we're using a pixel font here
      game.engineDisplay.SetDisplayLayer(EGameDisplayLayer.Game);
      game.engineDisplay.Begin();

      GameDisplay.Print("loading asset packs...", Vector2.One * 8, font);

      game.engineDisplay.End();

      // draw engine display to the screen
      graphicsDevice.SetRenderTarget(null);
      spriteBatch.Begin();
      Texture2D display = game.engineDisplay.ComposeLayers();
      spriteBatch.Draw(display, window.ClientBounds, Color.White);
      spriteBatch.End();
    }
  }
}
