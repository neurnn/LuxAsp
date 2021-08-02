using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp.Internals
{
    internal partial class LuxHostBuilder : ILuxHostBuilder, ILuxDryCaptureSettings
    {
        private List<Assembly> m_Assemblies = new List<Assembly>();
        private List<Assembly> m_StackedAssemblies = new List<Assembly>();

        private List<(int, Action<IConfiguration, IServiceCollection>)> m_Services;
        private List<(int, Action<IApplicationBuilder, IWebHostEnvironment>)> m_Configs;
        private List<(int, Action<IServiceProvider>)> m_Migrations;
        private List<(int, Action<IConfigurationBuilder>)> m_Settings;

        /// <summary>
        /// Initialize the New Lux Host builder here.
        /// </summary>
        /// <param name="HostBuilder"></param>
        public LuxHostBuilder(IHostBuilder HostBuilder)
        {
            m_Services = new List<(int, Action<IConfiguration, IServiceCollection>)>();
            m_Configs = new List<(int, Action<IApplicationBuilder, IWebHostEnvironment>)>();
            m_Migrations = new List<(int, Action<IServiceProvider>)>();
            m_Settings = new List<(int, Action<IConfigurationBuilder>)>();
            m_StackedAssemblies.Add(typeof(LuxHostBuilder).Assembly);

            HostBuilder
                 .ConfigureAppConfiguration(CaptureSettings)
                 .ConfigureWebHostDefaults(OnConfigureWebHostDefaults);
        }

        /// <summary>
        /// Test the application environment.
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<bool> TestEnvironment(Assembly Assembly = null)
        {
            Assembly ??= Assembly.GetEntryAssembly();
            yield return Debugger.IsAttached;

            var Attr = Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();

            if (Attr != null && !string.IsNullOrWhiteSpace(Attr.Configuration) &&
                string.Compare(Attr.Configuration, "Debug", true) == 0)
            {
                yield return true;
                yield break;
            }

            yield return Assembly /* Determines the entry assembly is debug mode or not.*/
                .GetCustomAttributes<DebuggableAttribute>()
                .Where(X => X.IsJITOptimizerDisabled).Count() > 0;

            var Env = Environment.GetEnvironmentVariable("LUXASP_ENVIRONMENT");
            yield return !string.IsNullOrWhiteSpace(Env) && string.Compare(Env, "Devel", true) == 0;

            Env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            yield return !string.IsNullOrWhiteSpace(Env) && string.Compare(Env, "Development", true) == 0;
        }

        /// <summary>
        /// Determines this host under development or not.
        /// </summary>
        public bool IsDevelopement { get; private set; } = TestEnvironment().Where(X => X).Count() > 0;

        /// <summary>
        /// Gets Root directory of the executable.
        /// </summary>
        public string ApplicationDirectory { get; private set; } = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        /// <summary>
        /// Host Builder's Properties that used until building host.
        /// </summary>
        public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

        /// <summary>
        /// Configure Default Settings for the application.
        /// </summary>
        /// <param name="Builder"></param>
        /// <param name="Configs"></param>
        internal static void ConfigureDefaultSettings(ILuxHostBuilder Builder, IConfigurationBuilder Configs)
        {
            Configs.AddJsonFile(Path.Combine(Builder.ApplicationDirectory, "appsettings.json"), true, true);
            if (Builder.IsDevelopement)
                Configs.AddJsonFile(Path.Combine(Builder.ApplicationDirectory, "appsettings.devel.json"), true, true);
        }

        /// <summary>
        /// Capture Settings.
        /// </summary>
        /// <param name="Builder"></param>
        public void CaptureSettings(IConfigurationBuilder Configs)
        {
            ConfigureDefaultSettings(this, Configs);

            foreach (var Each in m_Settings.OrderBy(X => X.Item1).Select(X => X.Item2))
                Each?.Invoke(Configs);
        }

        /// <summary>
        /// Called when the Lux WebHost Builder should be configured.
        /// </summary>
        /// <param name="Builder"></param>
        private void OnConfigureWebHostDefaults(IWebHostBuilder Builder)
        {
            var EntryAssembly = Assembly.GetEntryAssembly().GetName().Name;

            Builder
                .UseStartup(X => new Startup(X, this))
                .UseSetting(WebHostDefaults.ApplicationKey, EntryAssembly);
        }

        /// <summary>
        /// Add <see cref="Assembly"/> as Application Parts.
        /// </summary>
        /// <param name="Assemblies"></param>
        /// <returns></returns>
        public ILuxHostBuilder AddApplicationParts(params Assembly[] Assemblies)
        {
            lock (m_Assemblies)
            {
                Assemblies = Assemblies
                    .Where(X => !m_Assemblies.Contains(X))
                    .Where(X => !m_StackedAssemblies.Contains(X))
                    .ToArray();

                if (Assemblies.Length > 0)
                    m_Assemblies.AddRange(Assemblies);
            }

            return this;
        }

        /// <summary>
        /// Get Application Part assemblies.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Assembly> GetCurrentApplicationParts() => m_Assemblies;

        /// <summary>
        /// Get All Application Part Assemblies.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Assembly> GetApplicationParts() => m_StackedAssemblies;

        /// <summary>
        /// Clear Application Parts.
        /// </summary>
        public void CommitApplicationParts()
        {
            lock (m_StackedAssemblies)
            {
                m_StackedAssemblies.AddRange(m_Assemblies);
                m_Assemblies.Clear();
            }
        }

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
        /// Configure Services.
        /// </summary>
        /// <param name="Priority"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public ILuxHostBuilder ConfigureServices(int Priority, Action<IServiceCollection> Callback)
        {
            lock (m_Services)
                m_Services.Add((Priority, (_, Services) => Callback(Services)));

            return this;
        }

        /// <summary>
        /// Configure Services.
        /// </summary>
        /// <param name="Priority"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public ILuxHostBuilder ConfigureServices(int Priority, Action<IConfiguration, IServiceCollection> Callback)
        {
            lock (m_Services)
                m_Services.Add((Priority, Callback));

            return this;
        }

        /// <summary>
        /// Configure the Application.
        /// </summary>
        /// <param name="Priority"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public ILuxHostBuilder Configure(int Priority, Action<IApplicationBuilder, IWebHostEnvironment> Callback)
        {
            lock (m_Configs)
                m_Configs.Add((Priority, Callback));

            return this;
        }

        /// <summary>
        /// Configure Migrations.
        /// </summary>
        /// <param name="Priority"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public ILuxHostBuilder ConfigureMigrations(int Priority, Action<IServiceProvider> Callback)
        {
            lock (m_Migrations)
                m_Migrations.Add((Priority, Callback));

            return this;
        }

        /// <summary>
        /// Get Service Configurators.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Action<IConfiguration, IServiceCollection>> GetServices() 
            => m_Services.OrderBy(X => X.Item1).Select(X => X.Item2);

        /// <summary>
        /// Get Application Configurators.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Action<IApplicationBuilder, IWebHostEnvironment>> GetConfigs()
            => m_Configs.OrderBy(X => X.Item1).Select(X => X.Item2);

        /// <summary>
        /// Get Migrators. 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Action<IServiceProvider>> GetMigrations() 
            => m_Migrations.OrderBy(X => X.Item1).Select(X => X.Item2);

    }
}
