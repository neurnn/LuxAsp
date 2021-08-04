using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp.Internals
{
    internal class SettingsProvider : ISettingsProvider
    {
        private IServiceProvider m_Services;
        private Type[] m_EntityTypes;

        private Dictionary<Type, ISettingsRepository> m_Repositories
            = new Dictionary<Type, ISettingsRepository>();

        /// <summary>
        /// Initialize a new Lux Settings Provider.
        /// </summary>
        /// <param name="Services"></param>
        /// <param name="EntityTypes"></param>
        public SettingsProvider(IServiceProvider Services, IEnumerable<Type> EntityTypes)
        {
            m_Services = Services;
            m_EntityTypes = EntityTypes.ToArray();
        }

        /// <summary>
        /// Get Repository By Settings Model.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        private ISettingsRepository GetRepositoryByModel<TModel>() where TModel : SettingsModel
        {
            if (!m_EntityTypes.Contains(typeof(TModel)))
                return default;

            var Type = typeof(ISettingsRepository<>)
                .MakeGenericType(typeof(TModel));

            lock (m_Repositories)
            {
                if (!m_Repositories.TryGetValue(Type, out var _Repo))
                    m_Repositories[Type] = _Repo = m_Services.GetRequiredService(Type) as ISettingsRepository;

                return _Repo;
            }
        }

        /// <summary>
        /// Get Default Settings.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public TModel GetDefaultSettings<TModel>() where TModel : SettingsModel 
            => GetRepositoryByModel<TModel>().GetDefaultSettings(typeof(TModel)) as TModel;

        /// <summary>
        /// Get Settings by its Key.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="Key"></param>
        /// <returns></returns>
        public TModel GetSettings<TModel>(string Key) where TModel : SettingsModel
            => GetRepositoryByModel<TModel>().GetSettings(typeof(TModel), Key) as TModel;

        /// <summary>
        /// Set Default Settings.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="Value"></param>
        /// <returns></returns>
        public ISettingsProvider SetDefaultSettings<TModel>(TModel Value) where TModel : SettingsModel
        {
            GetRepositoryByModel<TModel>().SetDefaultSettings(typeof(TModel), Value);
            return this;
        }

        /// <summary>
        /// Set Settings.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public ISettingsProvider SetSettings<TModel>(string Key, TModel Value) where TModel : SettingsModel
        {
            GetRepositoryByModel<TModel>().SetSettings(typeof(TModel), Key, Value);
            return this;
        }
    }
}
