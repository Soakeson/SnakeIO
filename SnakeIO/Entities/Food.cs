using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Entities
{
    public class Food
    {
        private static MyRandom random = new MyRandom();
        public static Entity Create(Texture2D texture, Vector2 pos)
        {
            Entity food = new Entity();

            int r = random.Next(0, 255);
            int g = random.Next(0, 255);
            int b = random.Next(0, 255);

            food.Add(new Components.Renderable<Texture2D>(texture, new Color(r, g, b), Color.White));
            food.Add(new Components.Animatable(texture, new int[] { 100, 100, 100, 100, 100, 100 }));
            food.Add(new Components.Positionable(pos));
            food.Add(new Components.Consumable(typeof(Food), 1.0f));
            food.Add(new Components.Spawnable(TimeSpan.FromMilliseconds(5000), 5, typeof(Food)));
            Components.Animatable animatable = food.GetComponent<Components.Animatable>();
            int radius = animatable.subImageWidth >= animatable.spriteSheet.Height ? animatable.subImageWidth/2 : animatable.spriteSheet.Height/2;
            food.Add(new Components.Collidable(new Vector3(pos.X, pos.Y, radius)));
            food.Add(new Components.Movable(new Vector2(0, 0), new Vector2(0, 0)));

            return food;
        }
    }
}
