using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp.Internals
{
    internal class StorageConfiguration : LuxHostConfiguration, IStorageConfiguration
    {
        private Dictionary<string, Func<IServiceProvider, IStorage>> m_Registrations
            = new Dictionary<string, Func<IServiceProvider, IStorage>>();

        /// <summary>
        /// Mount a storage to the name what indicates. Note that the factory can be called multiple times.
        /// To make the storage as singleton, it should return same instance.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Factory"></param>
        /// <returns></returns>
        public IStorageConfiguration Mount(string Name, Func<IServiceProvider, IStorage> Factory)
        {
            m_Registrations[Name ?? "__DEFAULT__"] = Factory;
            return this;
        }

        /// <summary>
        /// Configure the Storage configurations to.
        /// </summary>
        /// <param name="Builder"></param>
        protected override void Configure(ILuxHostBuilder Builder)
        {
            Builder.AddApplicationParts(typeof(StorageConfiguration).Assembly);
            Builder.ConfigureServices(int.MinValue, Services =>
            {
                /* Add Storage Acessor instance. */
                Services.AddHttpService<IStorageProvider>(Context
                    => new StorageProvider(Context, m_Registrations));
            });
        }
    }
}
