using System;

namespace LuxAsp.Notations
{
    /// <summary>
    /// Make the property has unique value.
    /// </summary>
    public sealed class UniqueValue : PropertyNotationAttribute
    {
        protected override void Apply<TEntity, TProperty>(PropertyNotationContext<TEntity, TProperty> Context)
            => Context.NotationContext.Entity.HasIndex(Context.MutableProperty.Name);
    }
}
