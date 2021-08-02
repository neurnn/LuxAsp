using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LuxAsp.Internals
{
    internal class LuxDryHostBuilder : ILuxHostBuilder, ILuxDryCaptureSettings
    {
        private List<Assembly> m_ApplicationParts = new List<Assembly>();
        private List<(int, Action<IConfigurationBuilder>)> m_Settings
            = new List<(int, Action<IConfigurationBuilder>)>();

        public LuxDryHostBuilder(Assembly Entry)
        {
            IsDevelopement = LuxHostBuilder.TestEnvironment(Entry).Where(X => X).Count() > 0;
            ApplicationDirectory = Path.GetDirectoryName(Entry.Location);
        }

        /// <summary>
        /// Determines this host under development or not.
        /// </summary>
        public bool IsDevelopement { get; private set; }

        /// <summary>
        /// Gets Root directory of the executable.
        /// </summary>
        public string ApplicationDirectory { get; private set; }

        /// <summary>
        /// Host Builder's Properties that used until building host.
        /// </summary>
        public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

        /// <summary>
        /// Capture Settings.
        /// </summary>
        /// <param name="Builder"></param>
        public void CaptureSettings(IConfigurationBuilder Configs)
        {
            LuxHostBuilder.ConfigureDefaultSettings(this, Configs);

            foreach (var Each in m_Settings.OrderBy(X => X.Item1).Select(X => X.Item2))
                Each?.Invoke(Configs);
        }

        /// <summary>
        /// Add application part assemblies.
        /// </summary>
        /// <param name="Assemblies"></param>
        /// <returns></returns>
        public ILuxHostBuilder AddApplicationParts(params Assembly[] Assemblies)
        {
            foreach(var Each  in Assemblies)
            {
                if (m_ApplicationParts.Contains(Each))
                    continue;

                m_ApplicationParts.Add(Each);
            }

            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="Priority"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public ILuxHostBuilder Configure(int Priority, Action<IApplicationBuilder, IWebHostEnvironment> Callback) => this;

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="Priority"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public ILuxHostBuilder ConfigureMigrations(int Priority, Action<IServiceProvider> Callback) => this;

        /// <summary>
        /// Configure Settings.
        /// </summary>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public ILuxHostBuilder ConfigureSettings(int Priority, Action<IConfigurationBuilder> Callback)
        {
            lock (m_Settings)
                m_Settings.Add((Priority, Callback));

            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="Priority"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public ILuxHostBuilder ConfigureServices(int Priority, Action<IServiceCollection> Callback) => this;

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="Priority"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public ILuxHostBuilder ConfigureServices(int Priority, Action<IConfiguration, IServiceCollection> Callback) => this;

        /// <summary>
        /// Get Application Part assemblies.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Assembly> GetApplicationParts() => m_ApplicationParts;
    }
}
