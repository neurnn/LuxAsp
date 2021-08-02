using System.Collections;
using System.Collections.Generic;

namespace LuxAsp.Authorizations
{
    public class LuxAuthorizationKindCollection : ILuxAuthorizationKindCollection
    {
        private List<ILuxAuthorizationKind> m_Kinds = new List<ILuxAuthorizationKind>();

        /// <summary>
        /// Test whether the member has authorized or not.
        /// </summary>
        /// <typeparam name="TAuthorization"></typeparam>
        /// <returns></returns>
        public bool Has<TAuthorization>() where TAuthorization : ILuxAuthorizationKind
            => m_Kinds.FindIndex(X => X.GetType() == typeof(TAuthorization)) >= 0;

        /// <summary>
        /// Set authorization for the member.
        /// </summary>
        /// <param name="Authorization"></param>
        /// <returns></returns>
        public ILuxAuthorizationKindCollection Set(ILuxAuthorizationKind Authorization)
        {
            if (Authorization is null)
                return this;

            m_Kinds.RemoveAll(X => X.GetType() == Authorization.GetType());
            m_Kinds.Add(Authorization);
            return this;
        }

        /// <summary>
        /// Get authorization for the member.
        /// </summary>
        /// <typeparam name="TAuthorization"></typeparam>
        /// <returns></returns>
        public TAuthorization Get<TAuthorization>() where TAuthorization : ILuxAuthorizationKind
        {
            var Index = m_Kinds.FindIndex(X => X.GetType() == typeof(TAuthorization));
            if (Index >= 0) return (TAuthorization)m_Kinds[Index];
            return default;
        }

        /// <summary>
        /// Unset authorization.
        /// </summary>
        /// <typeparam name="TAuthorization"></typeparam>
        /// <returns></returns>
        public bool Unset<TAuthorization>() where TAuthorization : ILuxAuthorizationKind 
            => m_Kinds.RemoveAll(X => X.GetType() == typeof(TAuthorization)) > 0;

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ILuxAuthorizationKind> GetEnumerator() => m_Kinds.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_Kinds.GetEnumerator();
    }
}
