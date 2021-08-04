using Newtonsoft.Json;
using System;

namespace LuxAsp
{
    public abstract class SettingsModel
    {
        [JsonIgnore]
        private string m_Json;

        /// <summary>
        /// Set Json to settings model. (Internal Only)
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Value"></param>
        public static void SetJson(SettingsModel Model, string Value)
            => Model.m_Json = Value;

        /// <summary>
        /// Test whether the settings changed or not.
        /// </summary>
        public static bool DetectChanges(SettingsModel Model) 
            => Model != null && JsonConvert.SerializeObject(Model) != Model.m_Json;

        /// <summary>
        /// Called before saving the settings.
        /// </summary>
        public virtual void OnSave(IServiceProvider Services) { }
    }

}
