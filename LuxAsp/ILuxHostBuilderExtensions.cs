using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace LuxAsp
{
    public static class ILuxHostBuilderExtensions
    {
        /// <summary>
        /// Configure Settings.
        /// Note: appsettings.json and appsettings.devel.json file added defaultly.
        /// </summary>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public static ILuxHostBuilder ConfigureSettings(this ILuxHostBuilder This, Action<IConfigurationBuilder> Callback)
            => This.ConfigureSettings(LuxHostModule.Priority.Default, Callback);

        /// <summary>
        /// Configure Services as Default priority.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public static ILuxHostBuilder ConfigureServices(this ILuxHostBuilder This, Action<IServiceCollection> Callback)
            => This.ConfigureServices(LuxHostModule.Priority.Default, Callback);

        /// <summary>
        /// Configure the Application as Default priority.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public static ILuxHostBuilder Configure(this ILuxHostBuilder This, Action<IApplicationBuilder, IWebHostEnvironment> Callback)
            => This.Configure(LuxHostModule.Priority.Default, Callback);

        /// <summary>
        /// Configure the Migrations as Default priority.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public static ILuxHostBuilder ConfigureMigrations(this ILuxHostBuilder This, Action<IServiceProvider> Callback)
            => This.ConfigureMigrations(LuxHostModule.Priority.Default, Callback);

        /// <summary>
        /// Module State to manage module loading states.
        /// </summary>
        private class ModuleState : List<Type> { }

        /// <summary>
        /// Test whether the module type has been used.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="ModuleType"></param>
        /// <returns></returns>
        internal static bool IsModuleUse(this ILuxHostBuilder This, Type ModuleType)
        {
            ModuleState State;

            lock (This.Properties)
            {
                if (!This.Properties.ContainsKey(typeof(ModuleState)))
                    This.Properties[typeof(ModuleState)] = new ModuleState();

                State = This.Properties[typeof(ModuleState)] as ModuleState;
            }

            lock (State)
                return State.Contains(ModuleType);
        }

        /// <summary>
        /// Add Module Type that it has already used.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="ModuleType"></param>
        /// <returns></returns>
        internal static ILuxHostBuilder SetModuleUse(this ILuxHostBuilder This, Type ModuleType)
        {
            ModuleState State;

            lock (This.Properties)
            {
                if (!This.Properties.ContainsKey(typeof(ModuleState)))
                    This.Properties[typeof(ModuleState)] = new ModuleState();

                State = This.Properties[typeof(ModuleState)] as ModuleState;
            }

            lock (State)
            {
                if (State.Contains(ModuleType))
                    return This;

                State.Add(ModuleType);
            }

            return This;
        }

        /// <summary>
        /// Configure the Application using the Application Module.
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static ILuxHostBuilder UseModule(this ILuxHostBuilder This, Type ModuleType)
        {
            if (!ModuleType.IsSubclassOf(typeof(LuxHostModule)))
                throw new InvalidOperationException($"{ModuleType.FullName} isn't subclass of LuxHostModule.");

            var Ctor = ModuleType.GetConstructor(Type.EmptyTypes);
            if (Ctor is null)
                throw new InvalidOperationException($"{ModuleType.FullName} has no default constructor.");

            ModuleState State;

            lock (This.Properties)
            {
                if (!This.Properties.ContainsKey(typeof(ModuleState)))
                    This.Properties[typeof(ModuleState)] = new ModuleState();

                State = This.Properties[typeof(ModuleState)] as ModuleState;
            }

            lock (State)
            {
                if (State.Contains(ModuleType))
                    return This;

                State.Add(ModuleType);
            }

            (Ctor.Invoke(new object[0]) as LuxHostModule).InvokeConfigure(This);
            return This;
        }

        /// <summary>
        /// Configure the Application using the Application Module.
        /// </summary>
        /// <typeparam name="TModule"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static ILuxHostBuilder UseModule<TModule>(this ILuxHostBuilder This)
            where TModule : LuxHostModule, new()
        {
            ModuleState State;

            lock (This.Properties)
            {
                if (!This.Properties.ContainsKey(typeof(ModuleState)))
                    This.Properties[typeof(ModuleState)] = new ModuleState();

                State = This.Properties[typeof(ModuleState)] as ModuleState;
            }

            lock (State)
            {
                if (State.Contains(typeof(TModule)))
                    return This;

                State.Add(typeof(TModule));
            }

            (new TModule()).InvokeConfigure(This);
            return This;
        }

        /// <summary>
        /// Configure the Application using the Configuration object.
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <param name="This"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public static ILuxHostBuilder Use<TConfiguration>(this ILuxHostBuilder This, Action<TConfiguration> Callback = null)
            where TConfiguration : LuxHostConfiguration, new()
        {
            Action Invoker = null;
            bool IsCreated = false;

            lock(This.Properties)
            {
                if (!This.Properties.ContainsKey(typeof(TConfiguration)))
                {
                    This.Properties[typeof(TConfiguration)] = new TConfiguration();
                    IsCreated = true;
                }

                Invoker = () => Callback?.Invoke(This.Properties[typeof(TConfiguration)] as TConfiguration);
            }

            if (IsCreated)
            {
                (This.Properties[typeof(TConfiguration)] as TConfiguration)
                    .InvokeConfigure(This);
            }

            Invoker?.Invoke();
            return This;
        }

        /// <summary>
        /// Get Configuration from Host Builder.
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static TConfiguration GetConfiguration<TConfiguration>(this ILuxHostBuilder This)
            where TConfiguration : LuxHostConfiguration
        {
            lock (This.Properties)
            {
                if (!This.Properties.ContainsKey(typeof(TConfiguration)))
                    return default;

                return This.Properties[typeof(TConfiguration)] as TConfiguration;
            }
        }
    }
}
