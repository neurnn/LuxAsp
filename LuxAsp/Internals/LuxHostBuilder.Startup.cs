using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LuxAsp.Internals
{
    internal partial class LuxHostBuilder
    {
        private class Startup
        {
            private static readonly object[] EMPTY_ARGS = new object[0];
            private bool m_Initialized = false;

            /// <summary>
            /// Initialize the new Startup Instance.
            /// </summary>
            /// <param name="Context"></param>
            /// <param name="HostBuilder"></param>
            public Startup(WebHostBuilderContext Context, LuxHostBuilder HostBuilder)
            {
                /* Expose Configuration to. */
                this.HostBuilder = HostBuilder;
                Configuration = Context.Configuration;
            }

            /// <summary>
            /// Configuration.
            /// </summary>
            public IConfiguration Configuration { get; }

            /// <summary>
            /// Host Builder instance.
            /// </summary>
            public LuxHostBuilder HostBuilder { get; }

            /// <summary>
            /// Get Host Modules.
            /// </summary>
            private Startup Initialize()
            {
                lock (this)
                {
                    if (m_Initialized)
                        return this;

                    m_Initialized = true;
                }

                LuxHostModule.Invoke(HostBuilder);
                return this;
            }

            /// <summary>
            /// Configure the Application Services.
            /// </summary>
            /// <param name="Services"></param>
            public void ConfigureServices(IServiceCollection Services)
            {
                foreach (var Configure in Initialize().HostBuilder.GetServices())
                    Configure?.Invoke(Configuration, Services);
            }

            /// <summary>
            /// Configure the Application.
            /// </summary>
            /// <param name="App"></param>
            /// <param name="Env"></param>
            public void Configure(IApplicationBuilder App, IWebHostEnvironment Env)
            {
                foreach (var Configure in Initialize().HostBuilder.GetConfigs())
                    Configure?.Invoke(App, Env);

                Migrate(App);
            }

            /// <summary>
            /// Migrate the Application.
            /// </summary>
            /// <param name="App"></param>
            private void Migrate(IApplicationBuilder App)
            {
                var Factory = App.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
                using (var Scope = Factory.CreateScope())
                {
                    foreach (var Each in HostBuilder.GetMigrations())
                        Each?.Invoke(Scope.ServiceProvider);
                }
            }
        }
    }
}
