using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp.Internals
{
    internal class DatabaseOptions<TContext> : IDatabaseOptions, IDatabaseDryCreate
        where TContext : Database
    {
        private static readonly Type[] CTOR_WITH_SPEC_OPTIONS = new Type[] { typeof(DbContextOptions<TContext>) };
        private static readonly Type[] CTOR_WITH_OPTIONS = new Type[] { typeof(DbContextOptions) };

        private Action<IConfiguration, DbContextOptionsBuilder> m_Options;
        private DatabaseConfiguration m_Configs;

        /// <summary>
        /// Database Options.
        /// </summary>
        /// <param name="Configs"></param>
        public DatabaseOptions(DatabaseConfiguration Configs) => m_Configs = Configs;

        /// <summary>
        /// Configures the TContext as Database Context.
        /// </summary>
        /// <param name="Services"></param>
        public void Configure(IConfiguration Configuration, IServiceCollection Services)
        {
            if (m_Options is null)
                throw new InvalidOperationException("No database options configured!");

            Services.AddDbContextPool<TContext>(Options => ConfigureOptions(Configuration, Options));
            Services.AddScoped(Services => new DatabaseAccess(Services.GetRequiredService<TContext>()));
        }

        /// <summary>
        /// Configure options for the database.
        /// </summary>
        /// <param name="Options"></param>
        private void ConfigureOptions(IConfiguration Configuration, DbContextOptionsBuilder Options)
        {
            /* Add Database Extension to bypass entity types. */
            var Extension = Options.Options.FindExtension<DatabaseExtension>();
            if (Extension is null && Options is IDbContextOptionsBuilderInfrastructure _Options)
                _Options.AddOrUpdateExtension(Extension = new DatabaseExtension());

            if (Extension != null)
            {
                Extension.EntityTypes.Clear();
                Extension.EntityTypes.AddRange(
                    m_Configs.Repositories.Select(X => X.Entity));
            }

            m_Options?.Invoke(Configuration, Options);
        }

        /// <summary>
        /// Setup the context connection.
        /// </summary>
        /// <param name="Options"></param>
        /// <returns></returns>
        public IDatabaseOptions Setup(Action<IConfiguration, DbContextOptionsBuilder> Options)
        {
            m_Options = Options;
            return this;
        }

        /// <summary>
        /// Dry Create the Database.
        /// </summary>
        /// <returns></returns>
        Database IDatabaseDryCreate.DryCreate(IConfiguration Configuration) => DryCreate(Configuration);

        /// <summary>
        /// Dry Create the Database.
        /// </summary>
        /// <returns></returns>
        public TContext DryCreate(IConfiguration Configuration)
        {
            if (m_Options is null)
                throw new InvalidOperationException("No database options configured!");

            var Type = typeof(TContext);
            var Configs = new DbContextOptionsBuilder<TContext>();

            /* Configure the options builder. */
            ConfigureOptions(Configuration, Configs);

            var Constructor = Type
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Concat(Type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic))
                .Where(X =>
                {
                    var ParamTypes = X.GetParameters()
                        .Select(X => X.ParameterType)
                        .ToArray();

                    if (ParamTypes.Length <= 0) return false;
                    return CompareTypes(ParamTypes, CTOR_WITH_SPEC_OPTIONS)
                        || CompareTypes(ParamTypes, CTOR_WITH_OPTIONS);
                })
                .OrderBy(X => X.IsPublic ? 0 : 1)
                .FirstOrDefault();

            if (Constructor is null)
                throw new InvalidProgramException("the database doesn't define any valid constructor.");

            return Constructor.Invoke(new object[] { Configs.Options }) as TContext;
        }

        private bool CompareTypes(Type[] Left, Type[] Right)
        {
            if (Left.Length != Right.Length)
                return false;

            for(int i = 0; i < Left.Length; ++i)
            {
                if (Left[i].FullName != Right[i].FullName)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Add an entity type.
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <returns></returns>
        public IDatabaseOptions With<EntityType>() where EntityType : DataModel
        {
            Func<Repository> Factory = () => new DefaultRepository<EntityType>();

            lock (m_Configs)
            {
                m_Configs.Repositories
                .RemoveAll(X => X.Item1 == typeof(EntityType));

                m_Configs.Repositories.Add((typeof(EntityType), null, Factory));
            }

            return this;
        }

        /// <summary>
        /// Add an entity type with custom repository implementation.
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <typeparam name="RepoType"></typeparam>
        /// <returns></returns>
        public IDatabaseOptions With<EntityType, RepoType>()
             where EntityType : DataModel where RepoType : Repository<EntityType>, new()
        {
            Func<Repository> Factory = () => new RepoType();

            lock (m_Configs)
            {
                m_Configs.Repositories
                    .RemoveAll(X => X.Item1 == typeof(EntityType));

                m_Configs.Repositories.Add((typeof(EntityType), typeof(RepoType), Factory));
            }

            return this;
        }

    }
}
