using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations
{
    /// <summary>
    /// Authentication Information interface.
    /// </summary>
    public interface ILuxAuthentication : ILuxInfrastructure<HttpContext>, ILuxInfrastructure<IServiceProvider>
    {
        /// <summary>
        /// Determines the authentication is stateless or not.
        /// </summary>
        bool IsStateless { get; }

        /// <summary>
        /// Gets the Token if its stateless.
        /// </summary>
        LuxAuthenticationToken? Token { get; }

        /// <summary>
        /// Gets or Sets Authenticated Member.
        /// When the authentication method is based on stateless,
        /// This will cause NotSupportedException, so that,
        /// use Abandon method instead of assigning null to this property.
        /// </summary>
        ILuxAuthenticatedMember Member { get; set; }

        /// <summary>
        /// Create the Authentication Token based on currently authorized member.
        /// When not authenticated, this returns invalid token.
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        Task<LuxAuthenticationGeneratedToken> CreateAsync(CancellationToken Token = default);

        /// <summary>
        /// Refresh the Authentication Token using the Refresh Token.
        /// </summary>
        /// <param name="Refresh"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        Task<LuxAuthenticationGeneratedToken> RefreshAsync(LuxAuthenticationToken Refresh, CancellationToken Token = default);

        /// <summary>
        /// Abandon the authentication asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<bool> AbandonAsync(CancellationToken Token = default);
    }
}
