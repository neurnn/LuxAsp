using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LuxAsp
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add Http Service which created per Http Context.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="This"></param>
        /// <param name="Factory"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpService<TService>(
            this IServiceCollection This, Func<HttpContext, TService> Factory) where TService : class
        {
            This.AddScoped(Services =>
            {
                var Context = Services
                    .GetRequiredService<IHttpContextAccessor>()
                    .HttpContext;

                return Factory(Context);
            });

            return This;
        }

        /// <summary>
        /// Add Http Service which created per Http Context.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="This"></param>
        /// <param name="Factory"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpCleanableService<TService>(
            this IServiceCollection This, Func<HttpContext, Cleanable<TService>> Factory) where TService : class
        {
            This.AddScoped(Services =>
            {
                var Context = Services
                    .GetRequiredService<IHttpContextAccessor>()
                    .HttpContext;

                return Factory(Context);
            });

            This.AddScoped(Services => Services.GetRequiredService<Cleanable<TService>>().Instance);
            return This;
        }
    }
}
