using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace LuxAsp
{
    public abstract class NotationContext : IDisposable
    {
        /// <summary>
        /// Create Notation Context.
        /// </summary>
        /// <param name="Builder"></param>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static NotationContext Create(ModelBuilder Builder, IMutableEntityType Entity)
        {
            return typeof(NotationContext<>).MakeGenericType(Entity.ClrType)
                .GetConstructor(new Type[] { typeof(ModelBuilder), typeof(IMutableEntityType) })
                .Invoke(new object[] { Builder, Entity }) as NotationContext;
        }

        /// <summary>
        /// Properties that used for configuring model entity.
        /// </summary>
        public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

        /// <summary>
        /// Model Builder.
        /// </summary>
        public abstract ModelBuilder Model { get; }

        /// <summary>
        /// Entity Type Information.
        /// </summary>
        public abstract IMutableEntityType EntityType { get; }

        /// <summary>
        /// Clr Entity Type.
        /// </summary>
        public Type EntityClrType => EntityType.ClrType;

        /// <summary>
        /// Dispose the Notation Context.
        /// </summary>
        public abstract void Dispose();
    }

    /// <summary>
    /// Context that created when notation applied.
    /// </summary>
    public sealed class NotationContext<TEntity> : NotationContext where TEntity : class
    {
        private Queue<Action> m_Configures = new Queue<Action>();

        /// <summary>
        /// Initialize a new Notation Context using Builder and Entity Type.
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Entity"></param>
        public NotationContext(ModelBuilder Model, IMutableEntityType Entity)
        {
            this.Model = Model;
            this.EntityType = Entity;

            this.Entity = Model.Entity<TEntity>();
        }

        /// <summary>
        /// Model Builder.
        /// </summary>
        public override ModelBuilder Model { get; }

        /// <summary>
        /// Entity Type Information.
        /// </summary>
        public override IMutableEntityType EntityType { get; }

        /// <summary>
        /// Entity Builder.
        /// </summary>
        public EntityTypeBuilder<TEntity> Entity { get; }

        /// <summary>
        /// Configure a callback that called when this notation context disposed.
        /// </summary>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public NotationContext<TEntity> Configure(Action Callback)
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
