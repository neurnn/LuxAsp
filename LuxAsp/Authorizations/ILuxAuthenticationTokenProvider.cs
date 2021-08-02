using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations
{
    /// <summary>
    /// Authentication Token Provider.
    /// </summary>
    public interface ILuxAuthenticationTokenProvider
    {
        /// <summary>
        /// Http Request Instance.
        /// </summary>
        HttpRequest Request { get; }

        /// <summary>
        /// Create a new Token for authenticating the member.
        /// </summary>
        /// <param name="Member"></param>
        /// <param name="Cancellation"></param>
        /// <returns></returns>
        Task<LuxAuthenticationGeneratedToken> CreateAsync(ILuxAuthenticatedMember Member, CancellationToken Cancellation = default);

        /// <summary>
        /// Refresh the Token before its expiration.
        /// </summary>
        /// <param name="Member"></param>
        /// <param name="Token"></param>
        /// <param name="Cancellation"></param>
        /// <returns></returns>
        Task<LuxAuthenticationGeneratedToken> RefreshAsync(ILuxAuthenticatedMember Member, LuxAuthenticationToken Token, CancellationToken Cancellation = default);

        /// <summary>
        /// Resolve a Member from the authorization token.
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Cancellation"></param>
        /// <returns></returns>
        Task<ILuxAuthenticatedMember> ResolveAsync(LuxAuthenticationToken Token, CancellationToken Cancellation = default);

        /// <summary>
        /// Delete the authorization token.
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Cancellation"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(LuxAuthenticationToken Token, CancellationToken Cancellation = default);
    }
}
