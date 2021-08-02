using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuxAsp.LuxHostModule;

namespace LuxAsp.Internals
{
    internal class DatabaseConfiguration : LuxHostConfiguration
    {
        private Type m_ContextType;
        private object m_ContextOptions;

        private Action<IConfiguration, IServiceCollection> m_Configurator;

        /// <summary>
        /// Repository Types.
        /// </summary>
        public List<(Type Entity, Type Repo, Func<Repository> Factory)> Repositories { get; }
         = new List<(Type Entity, Type Repo, Func<Repository> Factory)>();

        /// <summary>
        /// Get Options instance.
        /// </summary>
        /// <returns></returns>
        public IDatabaseOptions GetOptions()
        {
            if (m_ContextOptions is null)
                return GetOptions<Database>();

            return m_ContextOptions as IDatabaseOptions;
        }

        /// <summary>
        /// Configure the Context.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public IDatabaseOptions GetOptions<TContext>() where TContext : Database
        {
            if (typeof(TContext) != m_ContextType)
            {
                var NewOptions = new DatabaseOptions<TContext>(this);

                m_ContextType = typeof(TContext);
                m_ContextOptions = NewOptions;

                m_Configurator = NewOptions.Configure;
                return NewOptions;
            }

            return m_ContextOptions as IDatabaseOptions;
        }

        /// <summary>
        /// Configure the Database.
        /// </summary>
        /// <param name="Builder"></param>
        protected override void Configure(ILuxHostBuilder Builder)
        {
            Builder.AddApplicationParts(typeof(DatabaseConfiguration).Assembly);
            Builder.ConfigureServices(Priority.Database,
                (Configs, Services) =>
                {
                    m_Configurator?.Invoke(Configs, Services);

                    foreach(var Each in Repositories)
                        AddRepositoryServices(Services, Each);
                });
        }

        /// <summary>
        /// Adds Repository Services.
        /// </summary>
        /// <param name="Services"></param>
        /// <param name="Each"></param>
        private void AddRepositoryServices(IServiceCollection Services, 
            (Type Entity, Type Repo, Func<Repository> Factory) Each)
        {
            var HolderType = typeof(RepositoryHolder<>).MakeGenericType(Each.Entity);
            var GenericRepo = typeof(Repository<>).MakeGenericType(Each.Entity);

            /* Add a repository as a Scoped Service. */
            Services.AddScoped(HolderType, Inner =>
            {
                return HolderType.GetConstructors().First()
                    .Invoke(new object[] { Each.Factory, Inner });
            });

            Services.AddScoped(GenericRepo, Services =>
            {
                var Holder = Services.GetRequiredService(HolderType)
                    as IRepositoryHolder;

                return Holder.Repository;
            });

            if (Each.Repo != null)
            {
                Services.AddScoped(Each.Repo, Services =>
                {
                    var Holder = Services.GetRequiredService(HolderType)
                        as IRepositoryHolder;

                    return Holder.Repository;
                });
            }
        }
    }
}