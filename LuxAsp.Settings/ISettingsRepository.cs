using System;

namespace LuxAsp
{
    public interface ISettingsRepository 
    {
        /// <summary>
        /// Get Default Settings.
        /// </summary>
        /// <returns></returns>
        SettingsModel GetDefaultSettings(Type ModelType);

        /// <summary>
        /// Get Non-Default Settings.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        SettingsModel GetSettings(Type ModelType, string Key);

        /// <summary>
        /// Set Default Settings for the Model.
        /// When null given, it should unset defaults.
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        ISettingsRepository SetDefaultSettings(Type ModelType, SettingsModel Value);

        /// <summary>
        /// Set Settings for the Model.
        /// When null given, it should unset settings.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        ISettingsRepository SetSettings(Type ModelType, string Key, SettingsModel Value);
    }

    public interface ISettingsRepository<TModel> : ISettingsRepository where TModel : SettingsModel
    {
        /// <summary>
        /// Get Default Settings.
        /// </summary>
        /// <returns></returns>
        TModel GetDefaultSettings();

        /// <summary>
        /// Get Non-Default Settings.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        TModel GetSettings(string Key);

        /// <summary>
        /// Set Default Settings for the Model.
        /// When null given, it should unset defaults.
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        ISettingsRepository<TModel> SetDefaultSettings(TModel Value);

        /// <summary>
        /// Set Settings for the Model.
        /// When null given, it should unset settings.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        ISettingsRepository<TModel> SetSettings(string Key, TModel Value);
    }
}
