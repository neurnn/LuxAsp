using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp.Notations
{
    /// <summary>
    /// Make the proerty as UTC date-time.
    /// </summary>
    public sealed class UniversalDateTime : PropertyNotationAttribute
    {
        private static ValueConverter Converter = new ValueConverter<DateTime, DateTime>(X => ToUtc(X), X => ToLocal(X));
        private static ValueComparer Comparer = new ValueComparer<DateTime>((X, Y) => ToUtc(X) == ToUtc(Y), X => X.GetHashCode(), X => ToLocal(X));

        protected override void Apply<TEntity, TProperty>(PropertyNotationContext<TEntity, TProperty> Context)
        {
            if (typeof(TProperty) != typeof(DateTime))
                return;

            Context.Property.HasConversion(Converter, Comparer);
        }

        /// <summary>
        /// Convert Input to UTC DateTime.
        /// </summary>
        /// <param name="In"></param>
        /// <returns></returns>
        private static DateTime ToUtc(DateTime In)
        {
            if (In.Kind != DateTimeKind.Utc)
                return In.ToUniversalTime();

            return In;
        }

        /// <summary>
        /// Convert Input to Local DateTime.
        /// </summary>
        /// <param name="In"></param>
        /// <returns></returns>
        private static DateTime ToLocal(DateTime In)
        {
            if (In.Kind != DateTimeKind.Local)
                return In.ToLocalTime();

            return In;
        }
    }
}
