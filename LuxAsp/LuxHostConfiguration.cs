namespace LuxAsp
{
    /// <summary>
    /// Base class for Lux Host Configurations.
    /// All classes that inherits from this can be only instantiated once.
    /// </summary>
    public abstract class LuxHostConfiguration
    {
        /// <summary>
        /// Invoke Configure method.
        /// </summary>
        /// <param name="Builder"></param>
        internal void InvokeConfigure(ILuxHostBuilder Builder) => Configure(Builder);

        /// <summary>
        /// Called when this host service should be configured.
        /// </summary>
        /// <param name="Builder"></param>
        protected abstract void Configure(ILuxHostBuilder Builder);
    }
}
