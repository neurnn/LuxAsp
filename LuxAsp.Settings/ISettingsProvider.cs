namespace LuxAsp
{
    public interface ISettingsProvider
    {
        /// <summary>
        /// Get Default Settings by Model.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        TModel GetDefaultSettings<TModel>() where TModel : SettingsModel;

        /// <summary>
        /// Get Settings By Model.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="Key"></param>
        /// <returns></returns>
        TModel GetSettings<TModel>(string Key) where TModel : SettingsModel;

        /// <summary>
        /// Set Default Settings.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="Value"></param>
        /// <returns></returns>
        ISettingsProvider SetDefaultSettings<TModel>(TModel Value) where TModel : SettingsModel;

        /// <summary>
        /// Set Settings By its Key.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        ISettingsProvider SetSettings<TModel>(string Key, TModel Value) where TModel : SettingsModel;
    }

}
