using LuxAsp.Implementations;
using LuxAsp.Internals;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LuxAsp
{
    public static class LuxStorageExtensions
    {
        /// <summary>
        /// Configure Storage for the application.
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static ILuxHostBuilder ConfigureStorage(this ILuxHostBuilder This, Action<IStorageConfiguration> Configure)
            => This.Use<StorageConfiguration>(X => Configure?.Invoke(X));

        /// <summary>
        /// Get Storage instance by its name.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static IStorage GetStorage(this IServiceProvider This, string Name = null)
        {
            var StorageProvider = This.GetRequiredService<IStorageProvider>();
            if (StorageProvider.TryGet(Name ?? "__DEFAULT__", out var Storage))
                return Storage;

            return null;
        }

        /// <summary>
        /// Get Required storage instance by its name.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static IStorage GetRequiredStorage(this IServiceProvider This, string Name = null)
            => This.GetStorage(Name) ?? throw new NotSupportedException($"No such storage registered: {Name}.");

        /// <summary>
        /// Mount File System to the name.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Name"></param>
        /// <param name="Directory"></param>
        /// <param name="BaseUrl"></param>
        /// <returns></returns>
        public static IStorageConfiguration MountFileSystem(this IStorageConfiguration This, string Name, DirectoryInfo Directory, string BaseUrl = null)
            => This.Mount(Name, _ => new FileSystemStorage(Directory, BaseUrl));
    }
}
