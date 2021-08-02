using System.Collections.Generic;
using System.Linq;

namespace LuxAsp.Authorizations
{
    public interface ILuxAuthorizationKindCollection : IEnumerable<ILuxAuthorizationKind>
    {
        /// <summary>
        /// Test whether the member has authorized or not.
        /// </summary>
        /// <typeparam name="TAuthorization"></typeparam>
        /// <returns></returns>
        bool Has<TAuthorization>() where TAuthorization : ILuxAuthorizationKind;

        /// <summary>
        /// Set authorization for the member.
        /// </summary>
        /// <param name="Authorization"></param>
        /// <returns></returns>
        ILuxAuthorizationKindCollection Set(ILuxAuthorizationKind Authorization);

        /// <summary>
        /// Get authorization for the member.
        /// </summary>
        /// <typeparam name="TAuthorization"></typeparam>
        /// <returns></returns>
        TAuthorization Get<TAuthorization>() where TAuthorization : ILuxAuthorizationKind;

        /// <summary>
        /// Unset authorization.
        /// </summary>
        /// <typeparam name="TAuthorization"></typeparam>
        /// <returns></returns>
        bool Unset<TAuthorization>() where TAuthorization : ILuxAuthorizationKind;
    }
}
