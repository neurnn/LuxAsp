using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Sessions
{
    public abstract class LuxSessionBase : ILuxSession
    {
        private Dictionary<string, byte[]> m_KeyValues = new Dictionary<string, byte[]>();

        /// <summary>
        /// Acquire and Release mechanism.
        /// </summary>
        public class AcquireAndRelease : ILuxSessionLockable
        {
            private TaskCompletionSource<bool> m_TCS;
            private async Task FromCancellationToken(CancellationToken Token)
            {
                var TCS = new TaskCompletionSource<bool>();
                if (Token.IsCancellationRequested)
                    return;

                using (Token.Register(() => TCS.TrySetResult(true)))
                    await TCS.Task;
            }

            /// <summary>
            /// Acquire the Session.
            /// </summary>
            /// <returns></returns>
            public virtual async Task AcquireAsync(CancellationToken Token)
            {
                while (true)
                {
                    Task Previous;

                    lock (this)
                    {
                        if (m_TCS is null)
                        {
                            m_TCS = new TaskCompletionSource<bool>();
                            return;
                        }

                        Previous = m_TCS.Task;
                    }

                    using var CTS = new CancellationTokenSource();
                    using (Token.Register(CTS.Cancel))
                    {
                        await Task.WhenAny(Previous, FromCancellationToken(CTS.Token));

                        if (!CTS.IsCancellationRequested) CTS.Cancel();
                        else throw new OperationCanceledException("Timeout reached until waiting the session.");
                    }
                }
            }

            /// <summary>
            /// Release the Session.
            /// </summary>
            public virtual Task ReleaseAsync(CancellationToken Token)
            {
                lock (this)
                {
                    m_TCS?.TrySetResult(true);
                    m_TCS = null;
                }

                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Session ID.
        /// </summary>
        public virtual string Id { get; protected set; }

        /// <summary>
        /// Lockable.
        /// </summary>
        public abstract ILuxSessionLockable Lockable { get; }

        /// <summary>
        /// Get Keys.
        /// </summary>
        public virtual IEnumerable<string> Keys => m_KeyValues.Keys;

        /// <summary>
        /// Abandon the Session.
        /// </summary>
        /// <returns></returns>
        public virtual bool Abandon()
        {
            lock (m_KeyValues)
                m_KeyValues.Clear();

            return true;
        }

        /// <summary>
        /// Get Value by its Key.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public virtual byte[] Get(string Key, Func<byte[]> Default = null)
        {
            lock (m_KeyValues)
            {
                if (m_KeyValues.TryGetValue(Key, out var Value))
                    return Value;

                if (Default != null && (Value = Default()) != null)
                    return m_KeyValues[Key] = Value;

                return null;
            }
        }

        /// <summary>
        /// Set Value by its Key.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public virtual ILuxSession Set(string Key, byte[] Value)
        {
            lock (m_KeyValues)
            {
                if (m_KeyValues.ContainsKey(Key))
                {
                    if (Value is null)
                    {
                        m_KeyValues.Remove(Key);
                        return this;
                    }
                }

                else if (Value != null)
                    m_KeyValues[Key] = Value;

                return this;
            }
        }

        /// <summary>
        /// Unset the Key.
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public virtual bool Unset(string Key)
        {
            lock (m_KeyValues)
                return m_KeyValues.Remove(Key);
        }
    }
}
