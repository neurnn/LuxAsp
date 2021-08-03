using LuxAsp.Sessions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        /// Get Service From the Service Collection.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="Services"></param>
        /// <returns></returns>
        private static TService GetServiceFromCollection<TService>(IServiceCollection Services) where TService : class
        {
            var LOD = Services.LastOrDefault(d => d.ServiceType == typeof(TService));

            if (LOD is null || LOD.ImplementationInstance is null)
                return default;

            return LOD.ImplementationInstance as TService;
        }

        /// <summary>
        /// Configure Default MVC Services.
        /// </summary>
        /// <param name="Builder"></param>
        private static void ConfigureMvcPattern(ILuxHostBuilder Builder)
        {
            Builder.ConfigureServices(int.MinValue, App =>
            {
                var AppParts = Builder.GetApplicationParts()
                    .Append(Assembly.GetEntryAssembly())
                    .GroupBy(X => X.FullName)
                    .SelectMany(X => X);

                var MvcBuilders = new IMvcBuilder[] {
                    App.AddControllers(),
                    App.AddRazorPages().AddRazorRuntimeCompilation()
                }.GroupBy(X => X).Select(X => X.Key);

                App.Configure<MvcRazorRuntimeCompilationOptions>(
                    Options =>
                    {
                        foreach (var AppPart in AppParts)
                            Options.FileProviders.Add(new EmbeddedFileProvider(AppPart));
                    });

                foreach (var Mvc in MvcBuilders)
                {
                    foreach (var AppPart in AppParts)
                        Mvc.AddApplicationPart(AppPart);

                    Mvc.AddNewtonsoftJson();
                }
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
