using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Sessions.Internals
{
    internal partial class LuxMemorySessionStore : LuxSessionStoreBase
    {
        private Dictionary<Guid, MemorySession> m_Instances = new Dictionary<Guid, MemorySession>();

        /// <summary>
        /// Initialize a new Memory Session Store.
        /// </summary>
        /// <param name="Options"></param>
        public LuxMemorySessionStore(LuxSessionOptions Options, ILuxSessionStoreWorker Worker) 
            : base(Options, Worker) { }

        /// <summary>
        /// Create a new Session asynchronously.
        /// When the Session supports Lockable, this should return with its locked.
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        protected override async Task<ILuxSession> CreateAsync(TimeSpan Expiration, CancellationToken Token)
        {
            var New = new MemorySession();
            await New.Lockable.AcquireAsync(Token);

            while (true)
            {
                New.Guid = Guid.NewGuid();

                lock (m_Instances)
                {
                    if (!m_Instances.ContainsKey(New.Guid))
                        return New;
                }
            }
        }

        /// <summary>
        /// Get Session.
        /// </summary>
        /// <param name="SGuid"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        protected override async Task<ILuxSession> GetAsync(string Id, TimeSpan Expiration, CancellationToken Token)
        {
            MemorySession Session;
            Guid SGuid;

            while (true)
            {
                if (!Guid.TryParse(Id, out SGuid))
                    return null;

                lock (m_Instances)
                {
                    if (m_Instances.TryGetValue(SGuid, out Session))
                        break;

                    if (TryRestoreSession(SGuid, out Session))
                    {
                        m_Instances[SGuid] = Session;
                        break;
                    }

                    return null;
                }
            }

            try { await Session.Lockable.AcquireAsync(Token); }
            catch { return null; }

            if ((DateTime.Now - Session.LastAccessTime) >= Expiration)
            {
                Session.Abandon();
                await DeleteAsync(Session, default);
                await Session.Lockable.ReleaseAsync(Token);
                return null;
            }

            Session.LastAccessTime = DateTime.Now;
            return  Session;
        }

        /// <summary>
        /// Try Restore Session from Somewhere when the session swapping implemented.
        /// When the Session restored successfully, this should delete the original file.
        /// </summary>
        /// <param name="Guid"></param>
        /// <param name="Session"></param>
        /// <returns></returns>
        protected virtual bool TryRestoreSession(Guid Guid, out MemorySession Session)
        {
            Session = null;
            return false;
        }

        /// <summary>
        /// Delete the Session asynchronously.
        /// When the Session supports Lockable, The session should be locked.
        /// </summary>
        /// <param name="Session"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        protected override Task DeleteAsync(ILuxSession Session, CancellationToken Token)
        {
            if (!(Session is MemorySession MemSession))
                return Task.CompletedTask;

            lock (m_Instances)
            {
                m_Instances.Remove(MemSession.Guid);
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Collect Garbages asynchronously.
        /// </summary>
        /// <returns></returns>
        protected override async ValueTask CollectAsync(TimeSpan Expiration)
        {
            MemorySession[] Sessions;

            lock (m_Instances)
            {
                var Temp = m_Instances
                    .Where(X => (DateTime.Now - X.Value.LastAccessTime) >= Expiration)
                    .Select(X => X.Value);

                if (Temp.Count() <= 0)
                    return;

                Sessions = Temp.ToArray();
                foreach (var Each in Sessions)
                    m_Instances.Remove(Each.Guid);
            }

            foreach (var Each in Sessions)
            {
                await Each.Lockable.AcquireAsync(default);
                Each.Abandon();

                await DeleteAsync(Each, default);
                await Each.Lockable.ReleaseAsync(default);
            }

            Sessions = null;
        }

        /// <summary>
        /// Swap Idle Sessions to Disk if implemented.
        /// </summary>
        /// <param name="IdleTimeout"></param>
        /// <returns></returns>
        protected override async ValueTask SwapIdlesAsync(TimeSpan IdleTimeout)
        {
            if (!SupportsIdleSwapping)
                return;

            MemorySession[] Sessions;
            IEnumerable<Task> Tasks;

            lock (m_Instances)
            {
                var Temp = m_Instances
                    .Where(X => (DateTime.Now - X.Value.LastAccessTime) >= IdleTimeout)
                    .Select(X => X.Value);

                if (Temp.Count() <= 0)
                    return;

                var Swappers = new List<Task>();
                Sessions = Temp.ToArray();

                foreach (var Each in Sessions)
                    Swappers.Add(SwapIdleAsync(Each));

                Tasks = Swappers;
            }

            await Task.WhenAll(Tasks);
            Sessions = null;
        }

        /// <summary>
        /// Swap the Idle Session.
        /// </summary>
        /// <param name="Each"></param>
        /// <returns></returns>
        private async Task SwapIdleAsync(MemorySession Each)
        {
            await Each.Lockable.AcquireAsync(default);

            lock (m_Instances)
            {
                if (TryStoreSession(Each.Guid, Each))
                    m_Instances.Remove(Each.Guid);
            }

            await Each.Lockable.ReleaseAsync(default);
        }

        /// <summary>
        /// Test whether the implementation supports Idle Swapping or not.
        /// </summary>
        /// <returns></returns>
        protected virtual bool SupportsIdleSwapping { get; } = false;

        /// <summary>
        /// Try to Store the Idle Session to Disk.
        /// </summary>
        /// <param name="Guid"></param>
        /// <param name="Session"></param>
        /// <returns></returns>
        protected virtual bool TryStoreSession(Guid Guid, MemorySession Session) => false;
    }
}
