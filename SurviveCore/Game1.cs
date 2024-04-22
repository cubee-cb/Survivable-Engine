using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SurviveCore.Engine;

namespace SurviveCore
{
  public class Game1 : Game
  {
    private GraphicsDeviceManager _graphics;
    private SpriteBatch spriteBatch;

    Texture2D texMissing;

    GameInstance placeholderGameInstance;


    public Game1()
    {
      _graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      IsMouseVisible = true;
    }



    protected override void Initialize()
    {
      // TODO: Add your initialization logic here
      placeholderGameInstance = new GameInstance(targetTickRate: 60, graphicsDevice: GraphicsDevice);

      base.Initialize();
    }



    protected override void LoadContent()
    {
      spriteBatch = new SpriteBatch(GraphicsDevice);

      // TODO: use this.Content to load your game content here
      texMissing = Content.Load<Texture2D>("tex/missing");

    }



    protected override void Update(GameTime gameTime)
    {
      float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

      // TODO: Add your update logic here
      ELDebug.Log("update!");
      placeholderGameInstance.Update(deltaTime);

      base.Update(gameTime);
    }



    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      // TODO: Add your drawing code here
      spriteBatch.Begin();

      placeholderGameInstance.Draw(spriteBatch);

      spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
