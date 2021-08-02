using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LuxAsp
{
    /// <summary>
    /// Extends HttpContext and HttpRequest.
    /// </summary>
    public static class HttpExtensions
    {
        /// <summary>
        /// Get Service from the Http Context.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static TService GetService<TService>(this HttpContext This)
            => This.RequestServices.GetService<TService>();

        /// <summary>
        /// Get Service from the Http Context.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static TService GetService<TService>(this HttpRequest This)
            => This.HttpContext.GetService<TService>();

        /// <summary>
        /// Get Service from the Http Context.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static TService GetRequiredService<TService>(this HttpContext This)
            => This.RequestServices.GetRequiredService<TService>();

        /// <summary>
        /// Get Service from the Http Context.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static TService GetRequiredService<TService>(this HttpRequest This)
            => This.HttpContext.GetRequiredService<TService>();
    }
}
