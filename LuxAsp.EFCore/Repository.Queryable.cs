using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LuxAsp
{

    public abstract partial class Repository<TEntity> where TEntity : DataModel
    {
        /// <summary>
        /// Wrapper for calling its loading event and setting bridge instance.
        /// </summary>
        private class Queryable : IQueryable<TEntity>
        {
            private IQueryable<TEntity> m_Original;
            private IBridge m_Bridge;

            public Queryable(IQueryable<TEntity> Original, IBridge Bridge)
            {
                m_Original = Original;
                m_Bridge = Bridge;
            }

            public Type ElementType => m_Original.ElementType;
            public Expression Expression => m_Original.Expression;
            public IQueryProvider Provider => m_Original.Provider;

            private class Enumerator : IEnumerator<TEntity>
            {
                private IEnumerator<TEntity> m_Original;
                private IBridge m_Bridge;

                public Enumerator(IEnumerator<TEntity> Original, IBridge Bridge)
                {
                    m_Original = Original;
                    m_Bridge = Bridge;
                }

                public TEntity Current => m_Original.Current;
                object IEnumerator.Current => m_Original.Current;

                public bool MoveNext()
                {
                    if (m_Original.MoveNext())
                    {
                        m_Original.Current.IsNew = false;
                        m_Original.Current.SetBridge(m_Bridge);
                        m_Original.Current.NotifyLoaded().Wait();

                        return true;
                    }

                    return false;
                }
                public void Dispose() => m_Original.Dispose();
                public void Reset() => m_Original.Reset();
            }

            public IEnumerator<TEntity> GetEnumerator() => new Enumerator(m_Original.GetEnumerator(), m_Bridge);
            IEnumerator IEnumerable.GetEnumerator() => new Enumerator(m_Original.GetEnumerator(), m_Bridge);
        }
    }
}
