using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Scenes
{
    public class GameScene : Scene
    {
        private SnakeIO.GameModel gameModel;
        private Systems.Renderer renderer;
        private Systems.KeyboardInput keyboardInput;
        private Systems.Selector<string> selector;
        private Systems.Audio audio;
        private Shared.Entities.Entity textInput;
        private Shared.Entities.Entity textBox;
        private ContentManager contentManager;
        private GameSceneState state = GameSceneState.Input;

        public GameScene(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, Shared.Controls.ControlManager controlManager)
        {
            this.Initialize(graphicsDevice, graphics, controlManager);
            this.controlManager = controlManager;
            this.keyboardInput = new Systems.KeyboardInput(controlManager);
            this.selector = new Systems.Selector<string>();
            this.renderer = new Systems.Renderer(spriteBatch);
            this.audio = new Systems.Audio();
        }

        override public void LoadContent(ContentManager contentManager)
        {
            this.contentManager = contentManager;
            SpriteFont font = contentManager.Load<SpriteFont>("Fonts/Micro5-50");
            Texture2D background = contentManager.Load<Texture2D>("Images/text-input-bkg");
            SoundEffect sound = contentManager.Load<SoundEffect>("Audio/click");
            string title = "ENTER YOUR NAME";
            this.textBox = Shared.Entities.TextBox.Create(font, title, new Rectangle((int)((screenWidth/2)-font.MeasureString(title).X), 50, 0, 0));
            AddEntity(textBox);
            this.textInput = Shared.Entities.TextInput.Create(
                        font, background, sound, "", 
                        true, new Rectangle((int)((screenWidth/2)-font.MeasureString("123").X),
                                            (int)((screenHeight/2)-font.MeasureString("1").Y),
                                            0, 0));
            AddEntity(textInput);
        }

        override public SceneContext ProcessInput(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return SceneContext.MainMenu;
            }

            selector.Update(gameTime.ElapsedGameTime);

            if (selector.hasSelected && selector.selectedVal != "")
            {
                RemoveEntity(this.textBox);
                state = GameSceneState.Game;
                StartGame(selector.ConsumeSelection());
            }

            return SceneContext.Game;
        }

        override public void Render(TimeSpan elapsedTime)
        {
            if (state == GameSceneState.Game)
            {
                gameModel.Render(elapsedTime);
            }
            else if (state == GameSceneState.Input)
            {
                renderer.Update(elapsedTime);
            }
        }

        override public void Update(TimeSpan elapsedTime)
        {
            if (state == GameSceneState.Game)
            {
                gameModel.Update(elapsedTime);
            }
            else if (state == GameSceneState.Input)
            {
                selector.Update(elapsedTime);
                audio.Update(elapsedTime);
                keyboardInput.Update(elapsedTime);
            }
        }

        public void StartGame(string name)
        {
            this.gameModel = new SnakeIO.GameModel(screenHeight, screenWidth);
            gameModel.Initialize(controlManager, spriteBatch, contentManager);
        }

        private void AddEntity(Shared.Entities.Entity entity)
        {
            renderer.Add(entity);
            selector.Add(entity);
            keyboardInput.Add(entity);
            audio.Add(entity);
        }

        private void RemoveEntity(Shared.Entities.Entity entity)
        {
            renderer.Remove(entity.id);
            selector.Remove(entity.id);
            keyboardInput.Remove(entity.id);
            audio.Remove(entity.id);
        }

        private enum GameSceneState
        {
            Input,
            Game
        }
    }
}

