using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Scenes
{
    public class GameScene : Scene
    {
        private SnakeIO.GameModel gameModel;
        public GameScene(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, Controls.ControlManager controlManager)
        {
            this.Initialize(graphicsDevice, graphics, controlManager);
            this.gameModel = new SnakeIO.GameModel(screenHeight, screenWidth);
        }

        override public void LoadContent(ContentManager contentManager)
        {
            gameModel.Initialize(controlManager, spriteBatch, contentManager);
        }

        override public SceneContext ProcessInput(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return SceneContext.MainMenu;
            }
            return SceneContext.Game;
        }

        override public void Render(GameTime gameTime)
        {
            gameModel.Render(gameTime);
        }

        override public void Update(GameTime gameTime)
        {
            gameModel.Update(gameTime);
        }
    }
}

