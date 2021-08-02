using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace LuxAsp
{
    public interface IDatabaseOptions
    {
        /// <summary>
        /// Setup the context connection.
        /// </summary>
        /// <param name="Options"></param>
        /// <returns></returns>
        IDatabaseOptions Setup(Action<IConfiguration, DbContextOptionsBuilder> Options);

        /// <summary>
        /// Add Entity Type without Repository Implementation.
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <returns></returns>
        IDatabaseOptions With<EntityType>() where EntityType : DataModel;

        /// <summary>
        /// Add Entity Type with Repository Implementation.
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <typeparam name="RepoType"></typeparam>
        /// <returns></returns>
        IDatabaseOptions With<EntityType, RepoType>()
             where EntityType : DataModel where RepoType : Repository<EntityType>, new();
    }
}