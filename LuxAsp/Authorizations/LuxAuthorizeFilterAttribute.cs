using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations
{
    /// <summary>
    /// Represents the controller an action requires authorization.
    /// </summary>
    public abstract class LuxAuthorizeFilterAttribute : LuxAuthenticateFilterAttribute
    {
        /// <summary>
        /// Called when the Controller or Action should be authenticated.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="Authentication"></param>
        /// <returns></returns>
        protected override async Task<bool> Authenticate(HttpContext Context, ILuxAuthentication Authentication)
        {
            if (await base.Authenticate(Context, Authentication))
                return await Authorize(Context, Authentication.Member.Authorizations);

            return false;
        }

        /// <summary>
        /// Called when authorization should be processed.
        /// Note: authorize filters can set authorization entities into member.
        /// When failed to authorize, this should return false.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="OutAuthorizations"></param>
        /// <returns></returns>
        protected abstract Task<bool> Authorize(HttpContext Context, ILuxAuthorizationKindCollection OutAuthorizations);
    }
}
