using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations
{
    public interface ILuxAuthenticationListener
    {
        /// <summary>
        /// Request Instance.
        /// </summary>
        HttpRequest Request { get; }

        /// <summary>
        /// Called when the member authorized.
        /// </summary>
        /// <param name="Member"></param>
        /// <param name="Cancellation"></param>
        /// <returns></returns>
        Task OnAuthorized(ILuxAuthenticatedMember Member, CancellationToken Cancellation = default);

        /// <summary>
        /// Called when the member unauthorized.
        /// </summary>
        /// <param name="Member"></param>
        /// <param name="Cancellation"></param>
        /// <returns></returns>
        Task OnUnauthorized(ILuxAuthenticatedMember Member, CancellationToken Cancellation = default);
    }
}
