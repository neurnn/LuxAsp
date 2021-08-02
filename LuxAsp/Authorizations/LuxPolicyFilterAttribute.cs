using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations
{
    /// <summary>
    /// Represents the controller an action requires policy.
    /// </summary>
    public abstract class LuxPolicyFilterAttribute : LuxAuthenticateFilterAttribute
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
                return await Enpolicy(Context, Authentication.Member.Policies);

            return false;
        }

        /// <summary>
        /// Called when en-policy should be processed.
        /// Note: policy filters can set policies into member.
        /// When failed to set policy, this should return false.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="OutPolices"></param>
        /// <returns></returns>
        protected abstract Task<bool> Enpolicy(HttpContext Context, ILuxPolicyKindCollection OutPolices);
    }
}
