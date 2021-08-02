using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LuxAsp
{
    public interface ILuxHostBuilder
    {
        /// <summary>
        /// Determines this host under development or not.
        /// </summary>
        bool IsDevelopement { get; }

        /// <summary>
        /// Gets Root directory of the executable.
        /// </summary>
        string ApplicationDirectory { get; }

        /// <summary>
        /// Host Builder's Properties that used until building host.
        /// </summary>
        IDictionary<object, object> Properties { get; }

        /// <summary>
        /// Add <see cref="Assembly"/> as Application Parts.
        /// </summary>
        /// <param name="Assemblies"></param>
        /// <returns></returns>
        ILuxHostBuilder AddApplicationParts(params Assembly[] Assemblies);

        /// <summary>
        /// Configure Settings.
        /// Note: appsettings.json and appsettings.devel.json file added defaultly.
        /// </summary>
        /// <param name="Callback"></param>
        /// <returns></returns>
        ILuxHostBuilder ConfigureSettings(int Priority, Action<IConfigurationBuilder> Callback);

        /// <summary>
        /// Configure Services.
        /// </summary>
        /// <param name="Callback"></param>
        /// <returns></returns>
        ILuxHostBuilder ConfigureServices(int Priority, Action<IServiceCollection> Callback);

        /// <summary>
        /// Configure Services.
        /// </summary>
        /// <param name="Callback"></param>
        /// <returns></returns>
        ILuxHostBuilder ConfigureServices(int Priority, Action<IConfiguration, IServiceCollection> Callback);

        /// <summary>
        /// Configure the Application.
        /// </summary>
        /// <param name="Callback"></param>
        /// <returns></returns>
        ILuxHostBuilder Configure(int Priority, Action<IApplicationBuilder, IWebHostEnvironment> Callback);

        /// <summary>
        /// Configure Migrations.
        /// </summary>
        /// <param name="Priority"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        ILuxHostBuilder ConfigureMigrations(int Priority, Action<IServiceProvider> Callback);

        /// <summary>
        /// Get Assembly Parts. (This method is only possible when Configure* series method)
        /// </summary>
        /// <returns></returns>
        IEnumerable<Assembly> GetApplicationParts();
    }
}
