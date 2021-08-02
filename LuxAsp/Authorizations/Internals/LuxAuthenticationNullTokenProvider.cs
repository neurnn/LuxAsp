using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations.Internals
{
    internal class LuxAuthenticationNullTokenProvider : ILuxAuthenticationTokenProvider
    {
        private static readonly Task<ILuxAuthenticatedMember> ALWAYS_NULL
            = Task.FromResult(null as ILuxAuthenticatedMember);

        private static readonly Task<LuxAuthenticationGeneratedToken> ALWAYS_INVALID
            = Task.FromResult(LuxAuthenticationGeneratedToken.Empty);

        private static readonly Task<bool> ALWAYS_FALSE = Task.FromResult(false);

        public LuxAuthenticationNullTokenProvider(HttpRequest Request)
            => this.Request = Request;

        public HttpRequest Request { get; }

        public Task<LuxAuthenticationGeneratedToken> CreateAsync(ILuxAuthenticatedMember Member, CancellationToken Cancellation = default) => ALWAYS_INVALID;
        public Task<bool> DeleteAsync(LuxAuthenticationToken Token, CancellationToken Cancellation = default) => ALWAYS_FALSE;
        public Task<LuxAuthenticationGeneratedToken> RefreshAsync(ILuxAuthenticatedMember Member, LuxAuthenticationToken Token, CancellationToken Cancellation = default) => ALWAYS_INVALID;
        public Task<ILuxAuthenticatedMember> ResolveAsync(LuxAuthenticationToken Token, CancellationToken Cancellation = default) => ALWAYS_NULL;
    }
}
