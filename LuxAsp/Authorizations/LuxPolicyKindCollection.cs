using System.Collections;
using System.Collections.Generic;

namespace LuxAsp.Authorizations
{
    public class LuxPolicyKindCollection : ILuxPolicyKindCollection
    {
        private List<ILuxPolicyKind> m_Kinds = new List<ILuxPolicyKind>();

        /// <summary>
        /// Test whether the member has policy or not.
        /// </summary>
        /// <typeparam name="TPolicy"></typeparam>
        /// <returns></returns>
        public bool Has<TPolicy>() where TPolicy : ILuxPolicyKind
            => m_Kinds.FindIndex(X => X.GetType() == typeof(TPolicy)) >= 0;

        /// <summary>
        /// Set policy for the member.
        /// </summary>
        /// <param name="Policy"></param>
        /// <returns></returns>
        public ILuxPolicyKindCollection Set(ILuxPolicyKind Policy)
        {
            if (Policy is null)
                return this;

            m_Kinds.RemoveAll(X => X.GetType() == Policy.GetType());
            m_Kinds.Add(Policy);
            return this;
        }

        /// <summary>
        /// Get policy for the member.
        /// </summary>
        /// <typeparam name="TPolicy"></typeparam>
        /// <returns></returns>
        public TPolicy Get<TPolicy>() where TPolicy : ILuxPolicyKind
        {
            var Index = m_Kinds.FindIndex(X => X.GetType() == typeof(TPolicy));
            if (Index >= 0) return (TPolicy)m_Kinds[Index];
            return default;
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ILuxPolicyKind> GetEnumerator() => m_Kinds.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_Kinds.GetEnumerator();
    }
}
