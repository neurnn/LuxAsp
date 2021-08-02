using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuxAsp.Internals
{
    /// <summary>
    /// Storage Accessor.
    /// These objects are instantiated per HttpContext.
    /// </summary>
    internal class StorageProvider : IStorageProvider
    {
        private Dictionary<string, Func<IServiceProvider, IStorage>> m_Factories;
        private Dictionary<string, IStorage> m_ActiveInstances;
        private HttpContext m_HttpContext;

        /// <summary>
        /// Initialize the Storage Accessor using factories.
        /// </summary>
        /// <param name="Factories"></param>
        public StorageProvider(HttpContext HttpContext, IDictionary<string, Func<IServiceProvider, IStorage>> Factories)
        {
            m_HttpContext = HttpContext;
            m_Factories = new Dictionary<string, Func<IServiceProvider, IStorage>>(Factories);
            m_ActiveInstances = new Dictionary<string, IStorage>();
        }

        /// <summary>
        /// Http Context instance.
        /// </summary>
        HttpContext ILuxInfrastructure<HttpContext>.Instance => m_HttpContext;

        /// <summary>
        /// IService Provider instance.
        /// </summary>
        IServiceProvider ILuxInfrastructure<IServiceProvider>.Instance => m_HttpContext.RequestServices;

        /// <summary>
        /// Try to get storage by its registration name.
        /// This will return false when the name couldn't be resolved to instance.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Storage"></param>
        /// <returns></returns>
        public bool TryGet(string Name, out IStorage Storage)
        {
            lock(m_ActiveInstances)
            {
                if (!m_ActiveInstances.TryGetValue(Name, out Storage))
                    return TryCreateInstance(Name, ref Storage);

                return true;
            }
        }

        /// <summary>
        /// Try to create the storage instance by its registration name.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Storage"></param>
        /// <returns></returns>
        private bool TryCreateInstance(string Name, ref IStorage Storage)
        {
            lock (m_ActiveInstances)
            {
                if (m_Factories.TryGetValue(Name, out var Factory))
                {
                    Storage = m_Factories[Name](m_HttpContext.RequestServices);
                    m_ActiveInstances[Name] = Storage;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Dispose the Storage accessor.
        /// </summary>
        public void Dispose()
        {
            while(true)
            {
                IStorage Storage;

                lock (m_ActiveInstances)
                {
                    var FirstKey = m_ActiveInstances.Keys.FirstOrDefault();
                    if (FirstKey is null)
                        break;

                    Storage = m_ActiveInstances[FirstKey];
                    m_ActiveInstances.Remove(FirstKey);
                }

                if (Storage is IDisposable _Disposable)
                    _Disposable.Dispose();

                else if (Storage is IAsyncDisposable _Async)
                    _Async.DisposeAsync().GetAwaiter().GetResult();
            }
        }

    }
}
