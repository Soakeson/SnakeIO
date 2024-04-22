using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Shared.Entities;
using System.Diagnostics;

namespace SnakeIO
{
    public class GameModel
    {
        public int HEIGHT { get; private set; }
        public int WIDTH { get; private set; }

        private Dictionary<uint, Entity> entities = new Dictionary<uint, Entity>(); // may not need

        private Systems.Renderer renderer;
        private Systems.KeyboardInput keyboardInput;
        private Systems.Network network;
        private Systems.Interpolation interpolation;
        private Systems.MouseInput mouseInput;
        private Shared.Systems.Linker linker;
        private Shared.Systems.Movement movement;
        private Systems.Audio audio;

        private ContentManager contentManager;
        private Shared.Controls.ControlManager controlManager;
        private Shared.Entities.Entity clientPlayer;

        public delegate void AddDelegate(Entity entity);
        private AddDelegate addEntity;

        private List<Entity> toRemove = new List<Entity>();
        private List<Entity> toAdd = new List<Entity>();

        public GameModel(int height, int width)
        {
            this.HEIGHT = height;
            this.WIDTH = width;
            addEntity = AddEntity;
        }

        public void Initialize(Shared.Controls.ControlManager controlManager, SpriteBatch spriteBatch, ContentManager contentManager)
        {
            this.renderer = new Systems.Renderer(spriteBatch);
            this.network = new Systems.Network();
            this.interpolation = new Systems.Interpolation();
            this.movement = new Shared.Systems.Movement();
            network.registerNewEntityHandler(handleNewEntity);
            network.registerRemoveEntityHandler(handleRemoveEntity);
            this.keyboardInput = new Systems.KeyboardInput(controlManager);
            this.mouseInput = new Systems.MouseInput(controlManager);
            this.audio = new Systems.Audio();
            this.linker = new Shared.Systems.Linker();
            this.contentManager = contentManager;
        }

        public void Update(TimeSpan elapsedTime)
        {
            network.update(elapsedTime, MessageQueueClient.instance.getMessages());
            keyboardInput.Update(elapsedTime);
            mouseInput.Update(elapsedTime);
            linker.Update(elapsedTime);
            movement.Update(elapsedTime);
            interpolation.Update(elapsedTime);
            audio.Update(elapsedTime);
        }

        public void Render(TimeSpan elapsedTime)
        {
            // DateTime startTime = DateTime.Now;
            renderer.Update(elapsedTime);
            // TimeSpan currentTime = DateTime.Now - startTime;
            // Console.WriteLine($"Render update time: {currentTime}");
        }

        private void AddEntity(Entity entity)
        {
            renderer.Add(entity);
            movement.Add(entity);
            keyboardInput.Add(entity);
            network.Add(entity);
            interpolation.Add(entity);
            mouseInput.Add(entity);
            audio.Add(entity);
            linker.Add(entity);

            entities[entity.id] = entity;
        }

        private void RemoveEntity(Entity entity)
        {
            renderer.Remove(entity.id);
            movement.Remove(entity.id);
            keyboardInput.Remove(entity.id);
            network.Remove(entity.id);
            interpolation.Remove(entity.id);
            mouseInput.Remove(entity.id);
            audio.Remove(entity.id);
            linker.Remove(entity.id);

            entities.Remove(entity.id);
        }

        private void RemoveEntity(uint id)
        {
            renderer.Remove(id);
            keyboardInput.Remove(id);
            network.Remove(id);
            interpolation.Remove(id);
            entities.Remove(id);
        }

        private void handleNewEntity(Shared.Messages.NewEntity message)
        {
            Entity entity = createEntity(message);
            AddEntity(entity);
        }

        /// <summary>
        /// Handler for the RemoveEntity message.  It removes the entity from
        /// the client game model (that's us!).
        /// </summary>
        private void handleRemoveEntity(Shared.Messages.RemoveEntity message)
        {
            RemoveEntity(message.id);
        }

        private Entity createEntity(Shared.Messages.NewEntity message)
        {
            Entity entity = new Entity(message.id);

            if (message.hasSnakeID)
            {
                entity.Add(new Shared.Components.SnakeID(message.snakeIDMessage.id));
            }

            if (message.hasReadable)
            {
                Rectangle rectangle = new Rectangle(
                        message.readableMessage.rectangle.X,
                        message.readableMessage.rectangle.Y,
                        message.readableMessage.rectangle.Width,
                        message.readableMessage.rectangle.Height);

                SpriteFont font = contentManager.Load<SpriteFont>(message.readableMessage.fontPath);
                entity.Add(new Shared.Components.Readable(message.readableMessage.text, message.readableMessage.color, message.readableMessage.stroke, rectangle, font: font));
            }

            if (message.hasRenderable)
            {
                Rectangle rectangle = new Rectangle(
                        message.renderableMessage.rectangle.X,
                        message.renderableMessage.rectangle.Y,
                        message.renderableMessage.rectangle.Width,
                        message.renderableMessage.rectangle.Height);

                Texture2D texture = contentManager.Load<Texture2D>(message.renderableMessage.texturePath);
                entity.Add(new Shared.Components.Renderable(message.readableMessage.color, message.readableMessage.stroke, rectangle, texture: texture));
            }

            if (message.hasAnimatable)
            {
                Shared.Components.Renderable renderable = entity.GetComponent<Shared.Components.Renderable>();
                entity.Add(new Shared.Components.Animatable(message.animatableMessage.spriteTime, renderable.texture));
            }

            if (message.hasPosition)
            {
                entity.Add(new Shared.Components.Positionable(new Vector2(message.positionableMessage.pos.X, message.positionableMessage.pos.Y), message.positionableMessage.orientation));
            }

            //TODO: find other ways to handle collidable. Maybe we specify what the radius is so that we don't have to calculate it.
            //There is no guaruntee that if it has position and has appearance that it will be collidable
            if (message.hasCollidable)
            {
                // Console.WriteLine($"{message.collidableMessage.Shape}, {message.collidableMessage.RectangleData.x}, {message.collidableMessage.RectangleData.y}, {message.collidableMessage.RectangleData.width}, {message.collidableMessage.RectangleData.height}");

                entity.Add(new Shared.Components.Collidable(message.collidableMessage.Shape, message.collidableMessage.RectangleData, message.collidableMessage.CircleData));
                // Shared.Components.Renderable renderable = entity.GetComponent<Shared.Components.Renderable>();
                // int radius = renderable.rectangle.Width >= renderable.rectangle.Height ? renderable.rectangle.Width / 2 : renderable.rectangle.Height / 2;
                // entity.Add(new Shared.Components.Collidable(new Vector3(message.positionableMessage.pos.X, message.positionableMessage.pos.Y, radius)));
            }

            if (message.hasMovement)
            {
                entity.Add(new Shared.Components.Movable(new Vector2(message.movableMessage.velocity.X, message.movableMessage.velocity.Y)));
            }

            if (message.hasSpawnable)
            {
                entity.Add(new Shared.Components.Spawnable(message.spawnableMessage.spawnRate, message.spawnableMessage.spawnCount, message.spawnableMessage.type));
            }

            if (message.hasConsumable)
            {
                entity.Add(new Shared.Components.Consumable(message.consumableMessage.growth));
            }

            if (message.hasGrowable)
            {
                entity.Add(new Shared.Components.Growable());
            }

            if (message.hasCamera)
            {
                Shared.Components.Positionable position = entity.GetComponent<Shared.Components.Positionable>();
                entity.Add(new Shared.Components.Camera(new Rectangle(message.cameraMessage.rectangle.X, message.cameraMessage.rectangle.Y, WIDTH, HEIGHT)));
                Shared.Components.Camera camera = entity.GetComponent<Shared.Components.Camera>();
            }

            if (message.hasLinkable)
            {
                entity.Add(new Shared.Components.Linkable(
                            message.linkableMessage.chain,
                            message.linkableMessage.linkPos
                            ));
                if (message.linkableMessage.linkPos != Shared.Components.LinkPosition.Head && message.hasPosition && message.hasMovement)
                {
                    entity.GetComponent<Shared.Components.Linkable>().linkDelegate = Shared.Entities.Body.BodyLinking;
                }
            }

            if (message.hasKeyboardControllable)
            {
                Shared.Components.Movable movable = entity.GetComponent<Shared.Components.Movable>();
                entity.Add(new Shared.Components.KeyboardControllable(message.keyboardControllableMessage.enable, message.keyboardControllableMessage.type, Shared.Entities.Player.PlayerKeyboardControls));
            }

            if (message.hasMouseControllable)
            {
                //Do Something
            }

            // This is the client's player
            if (message.hasKeyboardControllable || message.hasMouseControllable || message.hasCamera)
            {
                this.clientPlayer = entity;
            }

            return entity;
        }
    }
}
