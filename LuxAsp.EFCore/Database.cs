using LuxAsp.Internals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq;
using System.Reflection;

namespace LuxAsp
{
    /// <summary>
    /// Base class for DbContexts.
    /// </summary>
    public class Database : DbContext
    {
        private static readonly object[] MTD_ENTITY_ARGS = new object[0];
        private Type[] m_EntityTypes;

        /// <summary>
        /// Initialize a new Database Context.
        /// </summary>
        protected Database() => m_EntityTypes = Type.EmptyTypes;

        /// <summary>
        /// Initialize a new Database Context using Options.
        /// </summary>
        public Database(DbContextOptions Options)
            : base(Options)
        {
            m_EntityTypes = Options
                .GetExtension<DatabaseExtension>()
                .EntityTypes.ToArray();
        }

        /// <summary>
        /// Get Qualified Table Name.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public string GetQualifiedTableName<TEntity>() where TEntity : class
        {
            var Entity = Model.FindEntityType(typeof(TEntity));
            if (Entity != null)
                return Entity.GetSchemaQualifiedTableName();

            return null;
        }

        /// <summary>
        /// Called when Model Creating.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder Builder)
        {
            base.OnModelCreating(Builder);

            var AddMethod = typeof(ModelBuilder).GetMethods()
                .First(X => X.IsGenericMethod && X.Name == "Entity");

            /* Add Entity Types. */
            foreach (var EntityType in m_EntityTypes)
            {
                AddMethod
                    .MakeGenericMethod(EntityType)
                    .Invoke(Builder, MTD_ENTITY_ARGS);
            }

            /* Apply ModelPropertyAttributes. */
            foreach (var Entity in Builder.Model.GetEntityTypes())
            {
                if (Entity.ClrType != null)
                    OnModelCreating(Builder, Entity);
            }
        }

        /// <summary>
        /// Called when Model Entity configured.
        /// </summary>
        /// <param name="Builder"></param>
        /// <param name="Entity"></param>
        protected virtual void OnModelCreating(ModelBuilder Builder, IMutableEntityType Entity)
        {
            using (var Context = NotationContext.Create(Builder, Entity))
            {
                ApplyNotationAttributes(Entity, Context);
                ApplyPropertyNotationAttributes(Entity, Context);
            }
        }

        /// <summary>
        /// Apply all notation attributes.
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Context"></param>
        private static void ApplyNotationAttributes(IMutableEntityType Entity, NotationContext Context) 
            => NotationAttribute.InternalApply(Context, Entity.ClrType.GetCustomAttributes<NotationAttribute>(true));

        /// <summary>
        /// Apply all property notation attributes.
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="Context"></param>
        private static void ApplyPropertyNotationAttributes(IMutableEntityType Entity, NotationContext Context)
        {
            var Targets = Entity.GetProperties()
                .Where(X => X != null && X.PropertyInfo != null)
                .Select(X =>
                {
                    var PropertyNotations = X.PropertyInfo
                        .GetCustomAttributes<PropertyNotationAttribute>();

                    return (Property: X, Notations: PropertyNotations);
                });

            foreach (var Target in Targets)
            {
                var Property = Target.Property;
                var TEntity = Entity.ClrType;

                using (var PContext = PropertyNotationContext.Create(TEntity, Context, Property))
                    PropertyNotationAttribute.InternalApply(PContext, Target.Notations);
            }
        }
    }
}
