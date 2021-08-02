using System.Collections.Generic;

namespace LuxAsp.Authorizations
{
    public interface ILuxPolicyKindCollection : IEnumerable<ILuxPolicyKind>
    {
        /// <summary>
        /// Test whether the member has policy or not.
        /// </summary>
        /// <typeparam name="TPolicy"></typeparam>
        /// <returns></returns>
        bool Has<TPolicy>() where TPolicy : ILuxPolicyKind;

        /// <summary>
        /// Set policy for the member.
        /// </summary>
        /// <param name="Authorization"></param>
        /// <returns></returns>
        ILuxPolicyKindCollection Set(ILuxPolicyKind Authorization);

        /// <summary>
        /// Get policy for the member.
        /// </summary>
        /// <typeparam name="TPolicy"></typeparam>
        /// <returns></returns>
        TPolicy Get<TPolicy>() where TPolicy : ILuxPolicyKind;
    }
}
