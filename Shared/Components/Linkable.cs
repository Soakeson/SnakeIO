#nullable enable
using System;
using System.Text;

namespace Shared.Components 
{
    /// <summary>
    /// This component provides a one way link to some other component with the linkable component.
    /// The type of components that get linked will update the same type of component on the linked.
    /// If the linkedId is null that entity is linkable but not directly linked and affecting some other entity.
    /// This allows for other entities to link to it.
    /// </summary>
    public class Linkable : Component
    {
        public Entities.Entity nextEntity { get; set; }
        public Entities.Entity prevEntity { get; set; }
        public LinkPosition linkPos { get; set; }
        public LinkDelegate? linkDelegate { get; set; }
        public string chain { get; set; }

        public Linkable(string chain, LinkPosition linkPos, LinkDelegate? linkDelegate = null)
        {
            this.linkPos = linkPos;
            this.chain = chain;
            this.linkDelegate = linkDelegate;
        }

        public override void Serialize(ref List<byte> data)
        {
            data.AddRange(BitConverter.GetBytes(chain.Length));
            data.AddRange(Encoding.UTF8.GetBytes(chain));
            data.AddRange(BitConverter.GetBytes((UInt16)linkPos));
        }
    }

    public enum LinkPosition : UInt16
    {
        Head,
        Body,
        Tail
    }

    public delegate void LinkDelegate(Entities.Entity root);
}
