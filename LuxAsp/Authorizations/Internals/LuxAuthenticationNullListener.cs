using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations.Internals
{
    internal class LuxAuthenticationNullListener : ILuxAuthenticationListener
    {
        public LuxAuthenticationNullListener(HttpRequest Request)
            => this.Request = Request;

        public HttpRequest Request { get; }

        public Task OnAuthorized(ILuxAuthenticatedMember Member, CancellationToken Cancellation = default) => Task.CompletedTask;
        public Task OnUnauthorized(ILuxAuthenticatedMember Member, CancellationToken Cancellation = default) => Task.CompletedTask;
    }
}
