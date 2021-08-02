using LuxAsp.Internals;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace LuxAsp
{
    public static class ILuxHostBuilderExtensions
    {
        /// <summary>
        /// Configure Database.
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static ILuxHostBuilder ConfigureDatabase(this ILuxHostBuilder This, Action<IDatabaseOptions> Options) 
            => This.Use<DatabaseConfiguration>(Configs => Options?.Invoke(Configs.GetOptions()));

        /// <summary>
        /// Configure Database to use customized database class.
        /// </summary>
        /// <typeparam name="TDatabase"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static ILuxHostBuilder ConfigureDatabase<TDatabase>(this ILuxHostBuilder This, Action<IDatabaseOptions> Options)
            where TDatabase : Database => This.Use<DatabaseConfiguration>(Configs => Options?.Invoke(Configs.GetOptions<TDatabase>()));
    }
}
