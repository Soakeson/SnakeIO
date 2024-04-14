using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shared.Components
{
    /// <summary>
    /// This component is responsible for managing the state of Positionable entities.
    /// </summary>
    public class Positionable : Component
    {
        public Vector2 pos { get; set; }
        public Vector2 prevPos { get; set; }
        public float orientation { get; set; }

        public Positionable(Vector2 pos)
        {
            this.pos = pos;
            this.prevPos = pos;
            this.orientation = 0f;
        }

        public override void Serialize(ref List<byte> data)
        {
            data.AddRange(BitConverter.GetBytes(pos.X));
            data.AddRange(BitConverter.GetBytes(pos.Y));
            data.AddRange(BitConverter.GetBytes(orientation));
        }

    }
}
