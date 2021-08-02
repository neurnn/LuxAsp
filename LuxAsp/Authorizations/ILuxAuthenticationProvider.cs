using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations
{
    /// <summary>
    /// Authentication Provider.
    /// </summary>
    public interface ILuxAuthenticationProvider
    {
        /// <summary>
        /// Request Instance.
        /// </summary>
        HttpRequest Request { get; }

        /// <summary>
        /// Resolve the Member from its Guid that points it.
        /// </summary>
        /// <param name="Guid"></param>
        /// <param name="Cancellation"></param>
        /// <returns></returns>
        Task<ILuxAuthenticatedMember> ResolveAsync(Guid Guid, CancellationToken Cancellation = default);
    }
}
