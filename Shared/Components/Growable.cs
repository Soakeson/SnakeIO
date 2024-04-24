using System;
using System.Text;

namespace Shared.Components
{
    public class Growable : Component, IComparer<float>
    {
        public float growth { get; set; }
        public float prevGrowth { get; set; }

        public Growable()
        {
            this.growth = 0;
            this.prevGrowth = 0;
        }

        public void UpdateGrowth(float growth)
        {
            this.prevGrowth = this.growth;
            this.growth += growth;
        }

        public override void Serialize(ref List<byte> data)
        {
            data.AddRange(BitConverter.GetBytes(growth));
        }

        public int Compare(float a, float b)
        {
            return a.CompareTo(b);
        }
    }
}
