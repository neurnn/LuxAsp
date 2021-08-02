using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LuxAsp
{

    /// <summary>
    /// Notation attribute for marking model entity.
    /// </summary>
    public abstract class NotationAttribute : Attribute
    {
        /// <summary>
        /// Apply all model notations.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="Attributes"></param>
        internal static void InternalApply(NotationContext Context, IEnumerable<NotationAttribute> Attributes)
        {
            var Apply = typeof(NotationAttribute)
                .GetMethod("Apply", BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(Context.EntityClrType);

            foreach (var Each in Attributes)
                Apply.Invoke(Each, new object[] { Context });
        }

        /// <summary>
        /// Called when notation should be applied.
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="Builder"></param>
        /// <param name="Entity"></param>
        protected abstract void Apply<EntityType>(NotationContext<EntityType> Context) where EntityType : class;
    }
}
