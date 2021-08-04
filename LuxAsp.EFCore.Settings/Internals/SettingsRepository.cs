using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp.Internals
{
    internal class SettingsRepository : Repository<Settings>, ISettingsRepository
    {
        private Dictionary<string, Settings> m_Caches
            = new Dictionary<string, Settings>();

        /// <summary>
        /// Makes the cache key.
        /// </summary>
        /// <param name="ModelType"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        private string MakeCacheKey(Type ModelType, string Key) => $"{ModelType.Name}, {Key ?? ""}";

        /// <summary>
        /// Load Settings.
        /// </summary>
        /// <param name="ModelType"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        private Settings Load(Type ModelType, string Key = null)
        {
            Settings Settings;
            var CacheKey = MakeCacheKey(ModelType, Key);

            lock (m_Caches)
            {
                if (m_Caches.TryGetValue(CacheKey, out Settings))
                    return Settings;
            }

            var TypeName = ModelType.FullName;
            Settings = Load(Query(X => X.Type == TypeName)
                .Where(X => X.Key == ""))
                .FirstOrDefault();

            lock (m_Caches)
            {
                if (Settings != null)
                    m_Caches[CacheKey] = Settings;
            }

            return Settings;
        }

        private bool Uncache(Type ModelType, string Key)
        {
            var CacheKey = MakeCacheKey(ModelType, Key);

            lock (m_Caches)
                return m_Caches.Remove(CacheKey);
        }

        /// <summary>
        /// Get Default Settings.
        /// </summary>
        /// <param name="ModelType"></param>
        /// <returns></returns>
        public SettingsModel GetDefaultSettings(Type ModelType) => GetSettings(ModelType, null);

        /// <summary>
        /// Get Settings.
        /// </summary>
        /// <param name="ModelType"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public SettingsModel GetSettings(Type ModelType, string Key)
        {
            var Model = Load(ModelType, Key);
            if (Model != null)
            {
                if (Model.Instance is null)
                {
                    var Instance = !string.IsNullOrWhiteSpace(Model.Json) ? JsonConvert
                        .DeserializeObject(Model.Json, ModelType) as SettingsModel : null;

                    if (Instance != null)
                    {
                        Model.Instance = Instance;
                        SettingsModel.SetJson(Instance, Model.Json);
                    }
                }

                return Model.Instance;
            }

            return null;
        }

        /// <summary>
        /// Set Default Settings.
        /// </summary>
        /// <param name="ModelType"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public ISettingsRepository SetDefaultSettings(Type ModelType, SettingsModel Value) => SetSettings(ModelType, null, Value);

        /// <summary>
        /// Set Settings.
        /// </summary>
        /// <param name="ModelType"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public ISettingsRepository SetSettings(Type ModelType, string Key, SettingsModel Value)
        {
            Settings Settings;

            if (Value is null)
            {
                if ((Settings = Load(ModelType, Key)) != null)
                {
                    Uncache(ModelType, Key);
                    Settings.Delete();
                }

                return this;
            }

            while (true)
            {

                lock (m_Caches)
                {
                    if (m_Caches.TryGetValue(Key, out Settings)) 
                        break;
                }

                if ((Settings = Load(ModelType, Key)) is null)
                {
                    Settings = Create(() => new Settings
                    {
                        Type = ModelType.Name,
                        Key = Key ?? "",
                        Json = JsonConvert.SerializeObject(Value),
                        Instance = Value
                    });

                    Settings.Save();
                    return this;
                }

                break;
            }

            if (Settings.Instance != Value)
                Settings.Instance  = Value;

            if (SettingsModel.DetectChanges(Settings.Instance))
            {
                Settings.Json = JsonConvert.SerializeObject(Value);
                Settings.Save();

                SettingsModel.SetJson(Settings.Instance, Settings.Json);
            }

            return this;
        }

        protected override Task OnSaveRequest(Settings Entity, Func<Task<bool>> Next)
        {
            if (Entity.IsNew || Entity.IsChanged)
                Entity.LastWriteTime = DateTime.Now;

            return base.OnSaveRequest(Entity, Next);
        }
    }
}
