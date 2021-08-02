using System;

namespace LuxAsp
{
    /// <summary>
    /// Storage Mount interface.
    /// </summary>
    public interface IStorageConfiguration
    {
        /// <summary>
        /// Mount a storage to the name what indicates. Note that the factory can be called multiple times.
        /// To make the storage as singleton, it should return same instance.
        /// (When the instance inherits IDisposable interface, it will be get called per HttpContext)
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Factory"></param>
        /// <returns></returns>
        IStorageConfiguration Mount(string Name, Func<IServiceProvider, IStorage> Factory);
    }
}
