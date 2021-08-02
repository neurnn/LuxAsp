using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Sessions
{
    /// <summary>
    /// Session Store interface.
    /// </summary>
    public interface ILuxSessionStore
    {
        /// <summary>
        /// Open Session for the HttpContext asynchronously.
        /// When the context has expired or invalid session id, this should recreate it.
        /// And if this returns null for, it means that the session store couldn't create or load the session.
        /// </summary>
        /// <param name="HttpContext"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        Task<ILuxSession> OpenAsync(HttpContext HttpContext, CancellationToken Token = default);

        /// <summary>
        /// Close the Session asynchronously.
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        Task<bool> CloseAsync(ILuxSession Session, bool Abandon, CancellationToken Token = default);
    }
}
