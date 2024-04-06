using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Scenes
{
    public class MainMenuScene : Scene
    {
        private enum menuState
        {
            NewGame,
            HighScores,
            Controls,
            Credits,
            Exit
        }

        public MainMenuScene(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, Controls.ControlManager controlManager)
        {
            this.Initialize(graphicsDevice, graphics, controlManager);

        }

        override public void LoadContent(ContentManager contentManager)
        {

        }

        override public SceneContext ProcessInput(GameTime gameTime)
        {
            return SceneContext.MainMenu;
        }

        override public void Render(GameTime gameTime)
        {

        }

        override public void Update(GameTime gameTime)
        {

        }

    }
}
