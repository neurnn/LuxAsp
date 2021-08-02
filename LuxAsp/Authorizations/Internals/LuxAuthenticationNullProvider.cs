using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations.Internals
{
    internal class LuxAuthenticationNullProvider : ILuxAuthenticationProvider
    {
        private static readonly Task<ILuxAuthenticatedMember> ALWAYS_NULL
            = Task.FromResult(null as ILuxAuthenticatedMember);

        public LuxAuthenticationNullProvider(HttpRequest Request)
            => this.Request = Request;

        public HttpRequest Request { get; }

        /// <summary>
        /// Always return null.
        /// </summary>
        /// <param name="Guid"></param>
        /// <param name="Cancellation"></param>
        /// <returns></returns>
        public Task<ILuxAuthenticatedMember> ResolveAsync(Guid Guid, CancellationToken Cancellation = default)
            => ALWAYS_NULL;
    }
}
