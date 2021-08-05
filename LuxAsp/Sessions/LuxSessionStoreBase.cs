using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Sessions
{
    public abstract class LuxSessionStoreBase : ILuxSessionStore
    {
        private static AsyncLocal<HttpContext> m_Http = new AsyncLocal<HttpContext>();
        private CookieBuilder m_CookieBuilder;
        private LuxSessionOptions m_Options;
        private ILuxSessionStoreWorker m_Worker;

        /// <summary>
        /// Initialize a new Session Store.
        /// </summary>
        /// <param name="Options"></param>
        public LuxSessionStoreBase(LuxSessionOptions Options, ILuxSessionStoreWorker Worker)
        {
            (m_Options = Options).Cookies(m_CookieBuilder = new CookieBuilder());
            m_Worker = Worker;
        }

        /// <summary>
        /// Determines whether sessions shouldn't be locked or not.
        /// </summary>
        public bool NoLocks => m_Options.NoLocks;

        /// <summary>
        /// Gets or Sets Http Context.
        /// </summary>
        public HttpContext HttpContext => m_Http.Value;

        /// <summary>
        /// Open Session for the HttpContext asynchronously.
        /// When the context has expired or invalid session id, this should recreate it.
        /// And if this returns null for, it means that the session store couldn't create or load the session.
        /// </summary>
        /// <param name="HttpContext"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public async Task<ILuxSession> OpenAsync(HttpContext HttpContext, CancellationToken Token = default)
        {
            var Cookies = HttpContext.Request.Cookies;
            var OutCookies = HttpContext.Response.Cookies;
            ILuxSession Session; m_Http.Value = HttpContext;

            if (Cookies.TryGetValue(m_CookieBuilder.Name, out var Id) && !string.IsNullOrWhiteSpace(Id) &&
                (Session = await GetAsync(Id, m_Options.Expiration, Token)) != null)
                return Session;

            Session = await CreateAsync(m_Options.Expiration, Token);
            OutCookies.Append(m_CookieBuilder.Name, Session.Id, m_CookieBuilder.Build(HttpContext));
            return Session;
        }

        /// <summary>
        /// Close the Session asynchronously.
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public async Task<bool> CloseAsync(ILuxSession Session, bool Abandon, CancellationToken Token = default)
        {
            if (Session is null)
            {
                ExecutePassiveTasks();
                m_Http.Value = null;
                return false;
            }

            if (Abandon)
            {
                Session.Abandon();
                await DeleteAsync(Session, Token);
            }
            
            else await FlushAsync(Session, Token);
            if (Session.Lockable != null)
                await Session.Lockable.ReleaseAsync(Token);

            ExecutePassiveTasks();
            m_Http.Value = null;
            return true;
        }

        private DateTime m_IdleTime = DateTime.Now;
        private DateTime m_CollectTime = DateTime.Now;

        private ValueTask m_IdleTask = ValueTask.CompletedTask;
        private ValueTask m_CollectTask = ValueTask.CompletedTask;

        /// <summary>
        /// Executes a Passive Task.
        /// </summary>
        /// <param name="Term"></param>
        /// <param name="Task"></param>
        /// <param name="Time"></param>
        /// <param name="OutTask"></param>
        protected bool ExecutePassiveTask(TimeSpan Term, Func<ValueTask> Task, ref DateTime Time, ref ValueTask OutTask)
        {
            lock (this)
            {
                if (DateTime.Now - Time < Term)
                    return false;

                if (!OutTask.IsCompleted)
                    return false;

                Time = DateTime.Now;
                OutTask = m_Worker.ExecuteAsync(Task);
                return true;
            }
        }

        /// <summary>
        /// Execute Passive Tasks.
        /// </summary>
        protected virtual void ExecutePassiveTasks()
        {
            ExecutePassiveTask(m_Options.GCTimer, () => CollectAsync(m_Options.Expiration), ref m_CollectTime, ref m_CollectTask);
            ExecutePassiveTask(m_Options.IdleTimeout, () => SwapIdlesAsync(m_Options.IdleTimeout), ref m_IdleTime, ref m_IdleTask);
        }

        /// <summary>
        /// Create a new Session asynchronously.
        /// When the Session supports Lockable, this should return with its locked.
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        protected abstract Task<ILuxSession> CreateAsync(TimeSpan Expiration, CancellationToken Token);

        /// <summary>
        /// Get the Session asynchronously.
        /// When the Session supports Lockable, this should return with its locked.
        /// </summary>
        /// <param name="Cookies"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        protected abstract Task<ILuxSession> GetAsync(string Id, TimeSpan Expiration, CancellationToken Token);

        /// <summary>
        /// Delete the Session asynchronously.
        /// When the Session supports Lockable, The session should be locked.
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        protected abstract Task DeleteAsync(ILuxSession Session, CancellationToken Token);

        /// <summary>
        /// Flush Changes of the Session asynchronously.
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        protected virtual Task FlushAsync(ILuxSession Session, CancellationToken Token) => Task.CompletedTask;

        /// <summary>
        /// Collect Garbages asynchronously.
        /// </summary>
        /// <returns></returns>
        protected virtual ValueTask CollectAsync(TimeSpan Expiration) => ValueTask.CompletedTask;

        /// <summary>
        /// Swap Idle Tasks to Storage asynchronously.
        /// </summary>
        /// <returns></returns>
        protected virtual ValueTask SwapIdlesAsync(TimeSpan IdleTimeout) => ValueTask.CompletedTask;
    }
}
