using Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Entities
{
    /// <summary>
    /// A named entity that contains a collection of <see cref="Component"/> instances.
    /// </summary>
    public sealed class Entity
    {
        /// <summary>
        /// This entity's components.
        /// <para>This field is read-only. It should be added to using <see cref="Add(Component[])"/> and removed from
        /// using <see cref="Remove(Component)"/> or <see cref="Clear"/>, so this entity can update existing systems about changes made to it.</para>
        /// </summary>
        private readonly Dictionary<Type, Component> components = new Dictionary<Type, Component>();

        private static uint nextId = 0;
        /// <summary>
        /// Constructs a new <see cref="Entity"/> with the given name.
        /// is the preferred way to construct entities.</para>
        /// </summary>
        public Entity()
        {
            id = nextId++;
        }

        /// <summary>
        /// Gets the unique ID of this entity.
        /// </summary>
        public uint id { get; private set; }

        /// <summary>
        /// Returns whether this Entity contains a component of the given type.
        /// </summary>
        /// <param name="type">A type assignable to <see cref="Component"/> that this entity should check for in its component map.</param>
        /// <returns>Returns true if this entity contains a component of the given type, otherwise returns false.</returns>
        public bool ContainsComponent(Type type)
        {
            return components.ContainsKey(type) && components[type] != null;
        }

        /// <summary>
        /// Returns whether this Entity contains any component of the given type.
        /// </summary>
        public bool ContainsComponent<TComponent>()
            where TComponent : Component
        {
            return ContainsComponent(typeof(TComponent));
        }

        /// <summary>
        /// Allows a single component to be added
        /// </summary>
        /// <param name="component"></param>
        public void Add(Component component)
        {
            Debug.Assert(component != null, "component cannot be null");
            Debug.Assert(!this.components.ContainsKey(component.GetType()), "cannot add the same component twice");

            this.components.Add(component.GetType(), component);
        }

        /// <summary>
        /// Removes all components from this entity.
        /// </summary>
        public void Clear()
        {
            components.Clear();
        }

        /// <summary>
        /// Allows a single component to be removed
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        public void Remove<TComponent>()
            where TComponent : Component
        {
            this.components.Remove(typeof(TComponent));
        }

        /// <summary>
        /// Returns the component in this entity that is of the given type,
        /// </summary>        
        public TComponent GetComponent<TComponent>()
            where TComponent : Component
        {
            Debug.Assert(components.ContainsKey(typeof(TComponent)), string.Format("component of type {0} is not a part of this entity", typeof(TComponent)));
            return (TComponent)this.components[typeof(TComponent)];
        }

        /// <summary>
        /// Returns a human-friendly string representation of this entity, in the format of its name followed by a comma-separated
        /// list of its components' types.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}", id, string.Join(", ", from c in components.Values select c.GetType().Name));
        }
    }
}
