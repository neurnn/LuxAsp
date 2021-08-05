using LuxAsp.Sessions;
using LuxAsp.Sessions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp
{
    public static class LuxEFCoreSessionsExtensions
    {
        /// <summary>
        /// Adds the Session Model ans Repository.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static IDatabaseOptions WithEFSessions<TModel, TRepository>(this IDatabaseOptions This)
            where TModel : LuxSessionModel, new() where TRepository : LuxSessionRepository<TModel>, new()
        {
            return This.With<TModel, TRepository>();
        }

        /// <summary>
        /// Use EFCore for storing Sessions.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="This"></param>
        /// <returns></returns>
        public static LuxSessionOptions UseEFSessions<TModel, TRepository>(this LuxSessionOptions This)
            where TModel : LuxSessionModel, new() where TRepository : LuxSessionRepository<TModel>, new()
        {
            return This.Use<LuxEFCoreSessionStore<TModel, TRepository>>();
        }
    }
}
