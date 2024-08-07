using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Systems
{
    class Renderer : Shared.Systems.System
    {
        public SpriteBatch sb;
        public VertexPositionColor[] vertCircleStrip;
        public int[] indexCircleStrip;
        public BasicEffect effect;
        private Shared.Components.Camera? camera = null;
        private List<Shared.Entities.Entity> renderLast;

        public Renderer(SpriteBatch sb)
            : base(typeof(Shared.Components.Appearance))
        {
            this.sb = sb;

            this.renderLast = new List<Shared.Entities.Entity>();
            this.effect = new BasicEffect(sb.GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up),

                Projection = Matrix.CreateOrthographicOffCenter(
                                           0, sb.GraphicsDevice.Viewport.Width,
                                           sb.GraphicsDevice.Viewport.Height, 0,   // doing this to get it to match the default of upper left of (0, 0)
                                           0.1f, 2)
            };


        }

        public override void Update(TimeSpan elapsedTime)
        {
            foreach (var entity in entities.Values)
            {
                if (entity.ContainsComponent<Shared.Components.Camera>())
                {
                    camera = entity.GetComponent<Shared.Components.Camera>();
                }
                if (entity.ContainsComponent<Shared.Components.Invincible>())
                {
                    entity.GetComponent<Shared.Components.Invincible>().UpdateOpacity();
                }
                if (entity.ContainsComponent<Shared.Components.Animatable>())
                {
                    Shared.Components.Animatable animatable = entity.GetComponent<Shared.Components.Animatable>();
                    animatable.timeSinceLastFrame += elapsedTime;
                    if (animatable.timeSinceLastFrame > TimeSpan.FromMilliseconds(animatable.spriteTime[animatable.subImageIndex]))
                    {
                        animatable.timeSinceLastFrame -= TimeSpan.FromMilliseconds(animatable.spriteTime[animatable.subImageIndex]);
                        animatable.subImageIndex++;
                        animatable.subImageIndex = animatable.subImageIndex % animatable.spriteTime.Length;
                    }
                    if (camera != null)
                    {
                        camera.LerpAmount += camera.LerpSpeed;
                        camera.LerpAmount = camera.LerpAmount >= 1f ? 1f : camera.LerpAmount;
                        if (camera.ShouldRender(entity))
                        {
                            RenderAnimatable(entity);
                        }
                    }
                    else
                    {
                        RenderAnimatable(entity);
                    }
                    // RenderHitbox(entity);
                }
                else
                {
                    if (entity.ContainsComponent<Shared.Components.Renderable>())
                    {
                        if (camera != null)
                        {
                            camera.LerpAmount += camera.LerpSpeed;
                            camera.LerpAmount = camera.LerpAmount >= 1f ? 1f : camera.LerpAmount;
                            if (camera.ShouldRender(entity))
                            {
                                if (entity.ContainsComponent<Shared.Components.NameTag>())
                                {
                                    RenderTag(entity);
                                }
                                if (entity.ContainsComponent<Shared.Components.ParticleComponent>())
                                {
                                    RenderParticle(entity);
                                }
                                RenderEntity(entity);
                            }
                        }
                        else
                        {
                            RenderEntity(entity);
                        }
                    }
                    // RenderHitbox(entity);
                    if (entity.ContainsComponent<Shared.Components.Readable>())
                    {
                        RenderText(entity);
                    }

                }
            }

            RenderLast();
        }

        private void RenderEntity(Shared.Entities.Entity entity)
        {
            Shared.Components.Positionable positionable = entity.GetComponent<Shared.Components.Positionable>();
            Shared.Components.Renderable renderable = entity.GetComponent<Shared.Components.Renderable>();
            Color color = entity.ContainsComponent<Shared.Components.Invincible>() ? renderable.color * entity.GetComponent<Shared.Components.Invincible>().opacity : renderable.color;
            if (camera != null)
            {
                Matrix newMatrix = Matrix.Lerp(Matrix.Identity, camera.Transform, camera.LerpAmount);
                sb.Begin(transformMatrix: newMatrix);
            }
            else
            {
                sb.Begin();
            }
            if (entity.ContainsComponent<Shared.Components.Linkable>())
            {
                Shared.Components.Linkable link = entity.GetComponent<Shared.Components.Linkable>();
                if (link.linkPos == Shared.Components.LinkPosition.Head || link.linkPos == Shared.Components.LinkPosition.Tail)
                {
                    renderLast.Add(entity);
                }
            }
            sb.Draw(
                    renderable.texture,
                    new Rectangle(
                        (int)(positionable.pos.X - renderable.rectangle.Width / 2),
                        (int)(positionable.pos.Y - renderable.rectangle.Height / 2),
                        renderable.rectangle.Width,
                        renderable.rectangle.Height
                        ),
                    color
                   );
            sb.End();
        }

        private void RenderLast()
        {
            foreach (Shared.Entities.Entity entity in renderLast)
            {
                Shared.Components.Positionable positionable = entity.GetComponent<Shared.Components.Positionable>();
                Shared.Components.Renderable renderable = entity.GetComponent<Shared.Components.Renderable>();
                Color color = entity.ContainsComponent<Shared.Components.Invincible>() ? renderable.color * entity.GetComponent<Shared.Components.Invincible>().opacity : renderable.color;
                if (camera != null)
                {
                    Matrix newMatrix = Matrix.Lerp(Matrix.Identity, camera.Transform, camera.LerpAmount);
                    sb.Begin(transformMatrix: newMatrix);
                }
                else
                {
                    sb.Begin();
                }
                sb.Draw(
                        renderable.texture,
                        new Rectangle(
                            (int)(positionable.pos.X - renderable.rectangle.Width / 2),
                            (int)(positionable.pos.Y - renderable.rectangle.Height / 2),
                            renderable.rectangle.Width,
                            renderable.rectangle.Height
                            ),
                       color
                       );
                sb.End();
            }
            renderLast.Clear();
        }

        private void RenderText(Shared.Entities.Entity entity)
        {
            Shared.Components.Readable readable = entity.GetComponent<Shared.Components.Readable>();
            if (camera != null)
            {
                Matrix newMatrix = Matrix.Lerp(Matrix.Identity, camera.Transform, camera.LerpAmount);
                sb.Begin(transformMatrix: newMatrix);
            }
            else
            {
                sb.Begin();
            }
            DrawOutlineText(sb, readable.font, readable.text, readable.stroke, readable.color, 4, new Vector2(readable.rectangle.X, readable.rectangle.Y), readable.scale);
            sb.End();
        }

        private void RenderTag(Shared.Entities.Entity entity)
        {
            Shared.Components.NameTag nameTag = entity.GetComponent<Shared.Components.NameTag>();
            Shared.Components.Positionable positionable = entity.GetComponent<Shared.Components.Positionable>();
            if (camera != null)
            {
                Matrix newMatrix = Matrix.Lerp(Matrix.Identity, camera.Transform, camera.LerpAmount);
                sb.Begin(transformMatrix: newMatrix);
            }
            else
            {
                sb.Begin();
            }
            DrawOutlineText(sb, nameTag.font, nameTag.name, Color.White, Color.Black, 4, new Vector2(positionable.pos.X - 50, positionable.pos.Y - 75), .5f);
            sb.End();
        }

        private void RenderAnimatable(Shared.Entities.Entity entity)
        {
            Shared.Components.Positionable positionable = entity.GetComponent<Shared.Components.Positionable>();
            Shared.Components.Renderable renderable = entity.GetComponent<Shared.Components.Renderable>();
            Shared.Components.Animatable animatable = entity.GetComponent<Shared.Components.Animatable>();
            Color color = entity.ContainsComponent<Shared.Components.Invincible>() ? renderable.color * entity.GetComponent<Shared.Components.Invincible>().opacity : renderable.color;
            if (camera != null)
            {
                Matrix newMatrix = Matrix.Lerp(Matrix.Identity, camera.Transform, camera.LerpAmount);
                sb.Begin(transformMatrix: newMatrix);
            }
            else
            {
                sb.Begin();
            }
            sb.Draw(
                    animatable.spriteSheet,
                    new Rectangle(
                        (int)positionable.pos.X,
                        (int)positionable.pos.Y,
                        renderable.rectangle.Width,
                        renderable.rectangle.Height
                        ),
                    new Rectangle(animatable.subImageIndex * animatable.subImageWidth, 0, animatable.subImageWidth, animatable.spriteSheet.Height), // Source sub-texture
                    color,
                    positionable.orientation, // Angular rotation
                    new Vector2(animatable.subImageWidth / 2, animatable.spriteSheet.Height / 2), // Center point of rotation
                    SpriteEffects.None, 0);
            sb.End();
        }

        private void RenderHitbox(Shared.Entities.Entity entity)
        {
            Shared.Components.Positionable positionable = entity.GetComponent<Shared.Components.Positionable>();
            Shared.Components.Collidable collidable = entity.GetComponent<Shared.Components.Collidable>();
            indexCircleStrip = new int[360];
            vertCircleStrip = new VertexPositionColor[360];
            for (int i = 0; i < 360; i++)
            {
                indexCircleStrip[i] = i;
                vertCircleStrip[i].Position = new Vector3(Convert.ToSingle(positionable.pos.X + (collidable.Data.CircleData.radius * Math.Cos((float)i / 180 * Math.PI))), Convert.ToSingle(positionable.pos.Y + (collidable.Data.CircleData.radius * Math.Sin((float)i / 180 * Math.PI))), 0);
                vertCircleStrip[i].Color = Color.Red;
            }
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                sb.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.LineStrip,
                        vertCircleStrip, 0, vertCircleStrip.Length - 1,
                        indexCircleStrip, 0, indexCircleStrip.Length - 1
                        );
            }

        }

        private static void DrawOutlineText(SpriteBatch spriteBatch, SpriteFont font, string text, Color outlineColor, Color frontColor, int pixelOffset, Vector2 position, float scale)
        {
            // outline
            spriteBatch.DrawString(font, text, position - new Vector2(pixelOffset * scale, 0), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(pixelOffset * scale, 0), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position - new Vector2(0, pixelOffset * scale), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(0, pixelOffset * scale), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);

            // outline corners
            spriteBatch.DrawString(font, text, position - new Vector2(pixelOffset * scale, pixelOffset * scale), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(pixelOffset * scale, pixelOffset * scale), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position - new Vector2(-(pixelOffset * scale), pixelOffset * scale), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(-(pixelOffset * scale), pixelOffset * scale), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);

            // inside
            spriteBatch.DrawString(font, text, position, frontColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private void RenderParticle(Shared.Entities.Entity entity)
        {
            if (camera != null)
            {
                Matrix newMatrix = Matrix.Lerp(Matrix.Identity, camera.Transform, camera.LerpAmount);
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, transformMatrix: newMatrix);
            }
            else
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            }
            Shared.Components.ParticleComponent pComponent = entity.GetComponent<Shared.Components.ParticleComponent>();
            Shared.Components.Renderable renderable = entity.GetComponent<Shared.Components.Renderable>();

            sb.Draw(
                    renderable.texture,
                    new Rectangle(
                        (int)(pComponent.center.X),
                        (int)(pComponent.center.Y),
                        (int)pComponent.size.X,
                        (int)pComponent.size.Y
                        ),
                    null,
                    renderable.color,
                    pComponent.rotation,
                    new Vector2(renderable.texture.Width / 2, renderable.texture.Height / 2),
                    SpriteEffects.None,
                    0
                   );
            sb.End();
        }
    }
}
