using Microsoft.AspNetCore.Http;
using System;

namespace LuxAsp
{
    /// <summary>
    /// Storage Provider interface that allow to access to storage by its name.
    /// </summary>
    public interface IStorageProvider : ILuxInfrastructure<HttpContext>, ILuxInfrastructure<IServiceProvider>, IDisposable
    {
        /// <summary>
        /// Try to get storage by its name.
        /// This will return false when the name couldn't be resolved to instance.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Storage"></param>
        /// <returns></returns>
        bool TryGet(string Name, out IStorage Storage);
    }
}
