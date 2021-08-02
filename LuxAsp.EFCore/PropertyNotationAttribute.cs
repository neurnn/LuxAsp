using System;
using System.Collections.Generic;
using System.Reflection;

namespace LuxAsp
{
    /// <summary>
    /// Property Notation attribute for marking model properties.
    /// </summary>
    public abstract class PropertyNotationAttribute : Attribute
    {
        /// <summary>
        /// Apply all property notations.
        /// </summary>
        /// <param name="Notation"></param>
        /// <param name="Property"></param>
        /// <param name="PropertyNotations"></param>
        internal static void InternalApply(PropertyNotationContext Context, IEnumerable<PropertyNotationAttribute> PropertyNotations)
        {
            var Notation = Context.GetNotationContext();
            var Apply = typeof(PropertyNotationAttribute)
                .GetMethod("Apply", BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(Notation.EntityClrType, Context.PropertyType);

            foreach(var Each in PropertyNotations)
                Apply.Invoke(Each, new object[] { Context });
        }

        /// <summary>
        /// Called when property notation should be applied.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="Context"></param>
        protected abstract void Apply<TEntity, TProperty>(PropertyNotationContext<TEntity, TProperty> Context) where TEntity : class;
    }
}
