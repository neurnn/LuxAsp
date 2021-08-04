using LuxAsp.Internals;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace LuxAsp
{
    public class SettingsConfiguration : LuxHostConfiguration
    {
        private Dictionary<Type, Func<IServiceProvider, ISettingsRepository>>
            m_Factories = new Dictionary<Type, Func<IServiceProvider, ISettingsRepository>>();

        private Dictionary<Type, Type> m_Impls = new Dictionary<Type, Type>();

        /// <summary>
        /// Use Settings from.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="Factory"></param>
        /// <returns></returns>
        public SettingsConfiguration With<TModel, TRepository>(Func<IServiceProvider, TRepository> Factory = null)
            where TModel : SettingsModel where TRepository : ISettingsRepository<TModel>
        {
            var Type = typeof(TModel);

            m_Factories[Type] = Factory != null ? X => Factory(X) : null;
            m_Impls[Type] = typeof(TRepository);

            return this;
        }

        /// <summary>
        /// Use Settings from.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="Factory"></param>
        /// <returns></returns>
        public SettingsConfiguration With<TModel>(Func<IServiceProvider, ISettingsRepository> Factory)
        {
            var Type = typeof(TModel);

            m_Factories[Type] = Factory != null ? X => Factory(X) : throw new ArgumentNullException(nameof(Factory));
            m_Impls[Type] = null;

            return this;
        }

        /// <summary>
        /// Configure Settings Repositories to ServiceCollection.
        /// </summary>
        /// <param name="Builder"></param>
        protected override void Configure(ILuxHostBuilder Builder)
        {
            Builder.ConfigureServices(App =>
            {
                foreach (var Each in m_Factories.Keys)
                    AddSettingsRepository(App, Each);

                /* Add the Settings Provider. */
                App.AddScoped<ISettingsProvider>(
                    X => new SettingsProvider(X, m_Factories.Keys));
            });
        }

        /// <summary>
        /// Add Settings Repository.
        /// </summary>
        /// <param name="App"></param>
        /// <param name="Each"></param>
        private void AddSettingsRepository(IServiceCollection App, Type Type)
        {
            var ServiceType = typeof(ISettingsRepository<>).MakeGenericType(Type);

            if (m_Factories[Type] is null)
                 App.AddScoped(ServiceType, m_Impls[Type]);
            else App.AddScoped(ServiceType, X => m_Factories[Type](X));
        }
    }

}
