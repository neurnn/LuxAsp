using LuxAsp.Sessions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Internals
{
    [LuxHostModule(int.MinValue)]
    internal class LuxHostDefaults : LuxHostModule
    {
        private class CleanupCallbacks
        {
            public Queue<Func<Task>> Callbacks = new Queue<Func<Task>>();


        }

        /// <summary>
        /// Configure Lux Host Builder.
        /// </summary>
        /// <param name="Builder"></param>
        protected override void Configure(ILuxHostBuilder Builder)
        {
            ConfigureHttpFundamentals(Builder);
            ConfigureMvcPattern(Builder);

            /* Configure Endpoints. */
            Builder.Configure(int.MaxValue, (App, Env) =>
            {
                App.UseEndpoints(Endpoint =>
                {
                    Endpoint.MapRazorPages();
                    Endpoint.MapControllers();
                });
            });

        }

        /// <summary>
        /// Configure HTTP fundamental services.
        /// </summary>
        /// <param name="Builder"></param>
        private static void ConfigureHttpFundamentals(ILuxHostBuilder Builder)
        {
            /* Configure Http fundamentals. */
            Builder.ConfigureServices(int.MinValue, Services =>
            {
                /* Add Http Context Accessor. */
                Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            });
        }

        /// <summary>
        /// Configure Default MVC Services.
        /// </summary>
        /// <param name="Builder"></param>
        private static void ConfigureMvcPattern(ILuxHostBuilder Builder)
        {
            Builder.ConfigureServices(int.MinValue, App =>
            {
                var MvcBuilders = new IMvcBuilder[] {
                    App.AddControllers(),
                    App.AddRazorPages()
                }.GroupBy(X => X).Select(X => X.Key);

                var AppParts = Builder.GetApplicationParts();
                foreach (var AppPart in AppParts)
                {
                    foreach (var Mvc in MvcBuilders)
                        Mvc.AddApplicationPart(AppPart);
                }

                foreach (var Mvc in MvcBuilders)
                    Mvc.AddNewtonsoftJson();
            });

            /* Configure the authorization. */
            Builder.Configure(Priority.Between_Default_Late,
                (App, Env) =>
                {
                    App.UseRouting();
                    App.UseAuthorization();
                });
        }

    }
}
