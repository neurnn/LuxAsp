using System;
using System.Collections.Generic;
using System.Linq;

namespace LuxAsp.Notations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class MultiPrimaryKey : PropertyNotationAttribute
    {
        private class Collection<TEntity> : List<(string Property, int Order)> where TEntity : class
        {
            public static readonly object STATE_KEY = new object();
            public void Apply(NotationContext<TEntity> Context)
            {
                var Indices = this.GroupBy(X => X.Property);

                foreach (var Each in Indices)
                {
                    var Properties = Each
                        .OrderBy(X => X.Order)
                        .Select(X => X.Property)
                        .ToArray();

                    Context.Entity.HasKey(Properties);
                }
            }
        }

        public MultiPrimaryKey(int Order = 0)
            => this.Order = Order;

        public int Order { get; }

        /// <summary>
        /// Apply the Search-Index.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Context"></param>
        protected override void Apply<TEntity, TProperty>(PropertyNotationContext<TEntity, TProperty> Context)
            => Ensure(Context.NotationContext).Add((Context.MutableProperty.Name, Order));

        /// <summary>
        /// Gets or Create Search-Index Collection.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="Notation"></param>
        /// <returns></returns>
        private static Collection<TEntity> Ensure<TEntity>(NotationContext<TEntity> Notation) where TEntity : class
        {
            var STATE_KEY = Collection<TEntity>.STATE_KEY;

            if (!Notation.Properties.TryGetValue(STATE_KEY, out var _Object))
            {
                var Instance = new Collection<TEntity>();

                Notation.Properties[STATE_KEY] = Instance;
                Notation.Configure(() => Instance.Apply(Notation));

                return Instance;
            }

            return _Object as Collection<TEntity>;
        }
    }
}
