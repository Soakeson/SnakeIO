using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shared.Entities
{
    public class Particle
    {
        // This is the constructor used to kick off particle
        public static Entity Create(int? id, string texture, Rectangle rectangle, Color color, Shared.Components.ParticleComponent.ParticleType type, float orientation = 0)
        {
            Entity particle = new Entity();
            if (id != null)
            {
                particle.Add(new Shared.Components.SnakeID((int)id, ""));
            }
            particle.Add(new Shared.Components.Appearance(texture, typeof(Texture2D), color, color, rectangle));
            particle.Add(new Shared.Components.Positionable(new Vector2(rectangle.X, rectangle.Y), orientation));
            particle.Add(new Shared.Components.ParticleComponent(type, new Vector2(rectangle.X, rectangle.Y), true));

            return particle;
        }

        public static Entity Create(int? id, string texture, Rectangle rectangle, Color color, Shared.Components.ParticleComponent.ParticleType type, Vector2 center, Vector2 direction, float speed, Vector2 size, TimeSpan lifetime)
        {
            Entity particle = new Entity();
            if (id != null)
            {
                particle.Add(new Shared.Components.SnakeID((int)id, ""));
            }
            particle.Add(new Shared.Components.Appearance(texture, typeof(Texture2D), color, color, rectangle));
            particle.Add(new Shared.Components.Positionable(new Vector2(rectangle.X, rectangle.Y), 0));
            particle.Add(new Shared.Components.ParticleComponent(type, center, direction, speed, size, lifetime));

            return particle;
        }

    }
}
