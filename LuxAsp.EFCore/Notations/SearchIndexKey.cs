using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp.Notations
{
    /// <summary>
    /// Make the Search Index.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class SearchIndexKey : PropertyNotationAttribute
    {
        /// <summary>
        /// Search Index Collection.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        private class Collection<TEntity>
            : List<(string Name, int Order, string Property)> where TEntity : class
        {
            public static readonly object STATE_KEY = new object();
            public void Apply(NotationContext<TEntity> Context)
            {
                var Indices = this.GroupBy(X => X.Name);

                foreach (var Each in Indices)
                {
                    var Properties = Each
                        .OrderBy(X => X.Order)
                        .Select(X => X.Property)
                        .ToArray();

                    Context.Entity.HasIndex(Properties, Each.Key);
                }
            }
        }

        public SearchIndexKey(string Name, int Order = int.MaxValue)
        {
            this.Name = Name;
            this.Order = Order;
        }

        public string Name { get; }
        public int Order { get; }

        /// <summary>
        /// Apply the Search-Index.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Context"></param>
        protected override void Apply<TEntity, TProperty>(PropertyNotationContext<TEntity, TProperty> Context)
            => Ensure(Context.NotationContext).Add((Name, Order, Context.MutableProperty.Name));

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
