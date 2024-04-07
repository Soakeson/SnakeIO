using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Systems
{
    public class Linker : System
    {
        private Dictionary<string, List<uint>> chainHooks;

        public Linker()
            : base(
                    typeof(Components.Linkable)
                    )
        {
            this.chainHooks = new Dictionary<string, List<uint>>();
        }

        /// <summary>
        /// Overrides the system add function.
        /// Takes an entity that is interested in the Linkable component.
        /// Links it to the proper place in the proper chain depending on its properties.
        /// </summary>
        override public bool Add(Entities.Entity entity)
        {
            bool interested = IsInterested(entity);
            if (interested)
            {
                entities.Add(entity.id, entity);
                Link(entity);
            }

            return interested;
        }

        /// <summary>
        /// Links entity to its proper spot in its designated chain.
        /// </summary>
        private void Link(Entities.Entity entity)
        {
            Components.Linkable entityLink = entity.GetComponent<Components.Linkable>();

            // Create chain
            if (!chainHooks.ContainsKey(entityLink.chain))
            {
                List<uint> newChain = new List<uint>();
                chainHooks.Add(entityLink.chain, newChain);
            }

            List<uint> chain = chainHooks[entityLink.chain];

            if (chain.Count <= 0)
            {
                chain.Add(entity.id);
            }
            else if (entityLink.linkPos == Components.LinkPosition.Body)
            {
                Entities.Entity end = entities[chain[chain.Count-1]];
                // Insert before Tail
                if( end.GetComponent<Components.Linkable>().linkPos == Components.LinkPosition.Tail )
                {
                    Entities.Entity tail = end;
                    Components.Linkable tailLink = end.GetComponent<Components.Linkable>();
                    entityLink.prevEntity = tailLink.prevEntity;
                    entityLink.nextEntity = tail;
                    tailLink.prevEntity = entity;
                    chain.Add(entity.id);
                }
                else
                {
                    Components.Linkable endLink = end.GetComponent<Components.Linkable>();
                    endLink.nextEntity = entity;
                    entityLink.prevEntity = end;
                    chain.Add(entity.id);
                }
            }

            else if (entityLink.linkPos == Components.LinkPosition.Head)
            {
                Debug.Assert(entities[chain[0]].GetComponent<Components.Linkable>().linkPos != Components.LinkPosition.Head, "Head already attatched");
                Entities.Entity end = entities[chain[chain.Count-1]];
                // Link Head to Tail
                if (end.GetComponent<Components.Linkable>().linkPos == Components.LinkPosition.Tail)
                {
                    Entities.Entity next = entities[chain[0]];
                    Components.Linkable nextLink = next.GetComponent<Components.Linkable>();
                    Components.Linkable endLink = end.GetComponent<Components.Linkable>();
                    nextLink.prevEntity = entity;
                    entityLink.nextEntity = next;
                    entityLink.prevEntity = end;
                    endLink.nextEntity = entity;
                }
                else
                {
                    Entities.Entity next = entities[chain[0]];
                    Components.Linkable nextLink = next.GetComponent<Components.Linkable>();
                    nextLink.prevEntity = entity;
                    entityLink.nextEntity = next;
                }
                chain.Insert(0, entity.id);
            }

            else if (entityLink.linkPos == Components.LinkPosition.Tail)
            {
                Debug.Assert(entities[chain[chain.Count-1]].GetComponent<Components.Linkable>().linkPos != Components.LinkPosition.Tail, "Tail already attatched");
                Entities.Entity start = entities[chain[0]];
                // Link Tail to Head
                if (start.GetComponent<Components.Linkable>().linkPos == Components.LinkPosition.Head)
                {
                    Entities.Entity prev = entities[chain[chain.Count-1]];
                    Components.Linkable prevLink = prev.GetComponent<Components.Linkable>();
                    Components.Linkable startLink = start.GetComponent<Components.Linkable>();
                    entityLink.nextEntity = start;
                    entityLink.prevEntity = prev;
                    prevLink.nextEntity = entity;
                    startLink.prevEntity = entity;
                }
                else
                {
                    Entities.Entity prev = entities[chain[chain.Count-1]];
                    Components.Linkable prevLink = prev.GetComponent<Components.Linkable>();
                    prevLink.nextEntity = entity;
                    entityLink.prevEntity = prev;
                    chain.Add(entity.id);
                }
            }
        }

        //TODO: Linker Remove link function

        override public void Update(GameTime gameTime)
        {
            foreach (var entity in entities.Values)
            {
                LinkDelegate(entity);
            }
        }

        /// <summary>
        /// Runs the LinkDelegate function with itself and it's linked entity.
        /// </summary>
        private void LinkDelegate(Entities.Entity root)
        {
            Components.Linkable rootLink = root.GetComponent<Components.Linkable>();
            if (rootLink.linkDelegate != null)
            {
                rootLink.linkDelegate(root);
            }
        }
    }
}
