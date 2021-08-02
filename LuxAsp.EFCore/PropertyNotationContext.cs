using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace LuxAsp
{
    public abstract class PropertyNotationContext : IDisposable
    {
        /// <summary>
        /// Create a new property notation context.
        /// </summary>
        /// <param name="TEntity"></param>
        /// <param name="Context"></param>
        /// <param name="Property"></param>
        /// <returns></returns>
        public static PropertyNotationContext Create(Type TEntity, NotationContext Context, IMutableProperty Property)
        {
            var ContextType = typeof(NotationContext<>).MakeGenericType(TEntity);
            return typeof(PropertyNotationContext<,>)
                .MakeGenericType(TEntity, Property.PropertyInfo.PropertyType)
                .GetConstructor(new Type[] { ContextType, typeof(IMutableProperty) })
                .Invoke(new object[] { Context, Property }) as PropertyNotationContext;
        }

        /// <summary>
        /// Create a new property notation context.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="Context"></param>
        /// <param name="Property"></param>
        /// <returns></returns>
        public static PropertyNotationContext Create<TEntity>(NotationContext<TEntity> Context, IMutableProperty Property)
            where TEntity : class => Create(typeof(TEntity), Context, Property);

        /// <summary>
        /// Properties that used for configuring model entity.
        /// </summary>
        public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

        /// <summary>
        /// (Internal) GetNotationContext as non generic.
        /// </summary>
        /// <returns></returns>
        internal abstract NotationContext GetNotationContext();

        /// <summary>
        /// Gets Property Type.
        /// </summary>
        public abstract Type PropertyType { get; }

        /// <summary>
        /// Mutable Property.
        /// </summary>
        public abstract IMutableProperty MutableProperty { get; }

        /// <summary>
        /// Dispose the property notation context.
        /// </summary>
        public abstract void Dispose();
    }

    /// <summary>
    /// Property Notation Context.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public sealed class PropertyNotationContext<TEntity, TProperty> : PropertyNotationContext where TEntity : class
    {
        private Queue<Action> m_Configures = new Queue<Action>();

        public PropertyNotationContext(NotationContext<TEntity> Entity, IMutableProperty Property)
        {
            NotationContext = Entity;
            MutableProperty = Property;

            this.Property = Entity.Entity
                .Property<TProperty>(Property.Name);

            SetDefaults();
        }

        /// <summary>
        /// Set Defaults.
        /// </summary>
        private void SetDefaults()
        {
            if (typeof(TProperty) == typeof(Guid))
                Property.HasConversion<string>();
        }

        /// <summary>
        /// Gets Entity's Notation Context.
        /// </summary>
        public NotationContext<TEntity> NotationContext { get; }

        /// <summary>
        /// (Internal) GetNotationContext as non generic.
        /// </summary>
        /// <returns></returns>
        internal override NotationContext GetNotationContext() => NotationContext;

        /// <summary>
        /// Entity Builder.
        /// </summary>
        public EntityTypeBuilder<TEntity> Entity => NotationContext.Entity;

        /// <summary>
        /// Gets Property Builder.
        /// </summary>
        public PropertyBuilder<TProperty> Property { get; }

        /// <summary>
        /// Gets Property Type.
        /// </summary>
        public override Type PropertyType => typeof(TProperty);

        /// <summary>
        /// Mutable Property.
        /// </summary>
        public override IMutableProperty MutableProperty { get; }

        /// <summary>
        /// Configure a callback that called when this notation context disposed.
        /// </summary>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public PropertyNotationContext<TEntity, TProperty> Configure(Action Callback)
        {
            m_Configures.Enqueue(Callback);
            return this;
        }

        /// <summary>
        /// Dispose the context.
        /// </summary>
        public override void Dispose()
        {
            while (m_Configures.Count > 0)
                m_Configures.Dequeue()?.Invoke();
        }
    }
}
