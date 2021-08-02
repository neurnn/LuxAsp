using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuxAsp.Authorizations
{
    /// <summary>
    /// Represents the controller an action requires authentication.
    /// </summary>
    public class LuxAuthenticateFilterAttribute : ActionFilterAttribute
    {
        private static readonly Task<IActionResult> NULL_RESULT = Task.FromResult(null as IActionResult);

        /// <summary>
        /// Authenticates the Controller or Action before executing it.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="Next"></param>
        /// <returns></returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext Context, ActionExecutionDelegate Next)
        {
            var Authentication = Context.HttpContext.GetRequiredService<ILuxAuthentication>();

            if (await Authenticate(Context.HttpContext, Authentication))
                await base.OnActionExecutionAsync(Context, Next);

            else if ((Context.Result = await OnFailoverAsync(Context.HttpContext)) is null)
                Context.Result = new StatusCodeResult(401);
        }

        /// <summary>
        /// Authenticates the Controller or Action before executing it.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="Next"></param>
        /// <returns></returns>
        public override async Task OnResultExecutionAsync(ResultExecutingContext Context, ResultExecutionDelegate Next)
        {
            var Authentication = Context.HttpContext.GetRequiredService<ILuxAuthentication>();

            if (await Authenticate(Context.HttpContext, Authentication))
                await base.OnResultExecutionAsync(Context, Next);

            else if ((Context.Result = await OnFailoverAsync(Context.HttpContext)) is null)
                Context.Result = new StatusCodeResult(401);
        }

        /// <summary>
        /// Called when the Controller or Action should be authenticated.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="Authentication"></param>
        /// <returns></returns>
        protected virtual Task<bool> Authenticate(HttpContext Context, ILuxAuthentication Authentication)
            => Task.FromResult(Authentication != null && Authentication.Member != null);

        /// <summary>
        /// Called when failed to authenticate.
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        protected virtual Task<IActionResult> OnFailoverAsync(HttpContext Context) => NULL_RESULT;
    }
}
