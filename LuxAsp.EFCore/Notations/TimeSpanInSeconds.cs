using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace LuxAsp.Notations
{
    /// <summary>
    /// Make the TimeSpan property to be integeric and seconds only.
    /// </summary>
    public sealed class TimeSpanInSeconds : PropertyNotationAttribute
    {
        protected override void Apply<TEntity, TProperty>(PropertyNotationContext<TEntity, TProperty> Context)
        {
            if (Context is PropertyNotationContext<TEntity, TimeSpan> _Context)
                Apply(_Context);
        }

        private void Apply<TEntity>(PropertyNotationContext<TEntity, TimeSpan> Context) where TEntity : class
        {
            Context.Property.HasConversion(
                X => (long)X.TotalSeconds, X => TimeSpan.FromSeconds(X),
                new ValueComparer<int>((X, Y) => X == Y, (X) => X.GetHashCode()));
        }
    }
}
