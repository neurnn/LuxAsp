using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuxAsp.Sessions
{
    /// <summary>
    /// Represents the controller or action requires session.
    /// </summary>
    public class LuxSessionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Sessionize the Controller or Action before executing it.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="Next"></param>
        /// <returns></returns>
        public override Task OnActionExecutionAsync(ActionExecutingContext Context, ActionExecutionDelegate Next)
            => OnActionSessionExecutionAsync(Context.HttpContext.GetRequiredService<ILuxSession>(), Context, Next);

        /// <summary>
        /// Sessionize the Controller or Action before executing it.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="Next"></param>
        /// <returns></returns>
        public override Task OnResultExecutionAsync(ResultExecutingContext Context, ResultExecutionDelegate Next)
            => OnResultSessionExecutionAsync(Context.HttpContext.Request.GetRequiredService<ILuxSession>(), Context, Next);

        /// <summary>
        /// Called when the Controller or Action is sessionized.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="Next"></param>
        /// <returns></returns>
        protected virtual Task OnActionSessionExecutionAsync(ILuxSession Session, ActionExecutingContext Context, ActionExecutionDelegate Next) 
            => base.OnActionExecutionAsync(Context, Next);

        /// <summary>
        /// Called when the Controller or Action is sessionized.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="Next"></param>
        /// <returns></returns>
        protected virtual Task OnResultSessionExecutionAsync(ILuxSession Session, ResultExecutingContext Context, ResultExecutionDelegate Next) 
            => base.OnResultExecutionAsync(Context, Next);
    }
}
