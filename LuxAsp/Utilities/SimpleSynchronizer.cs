using LuxAsp.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Utilities
{
    public sealed class SimpleSynchronizer : IDisposable
    {
        private SemaphoreSlim m_Semaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Wait the Synchronizer asynchronously.
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public async Task<IDisposable> WaitAsync(CancellationToken Token = default)
        {
            try { await m_Semaphore.WaitAsync(Token); }
            catch
            {
                Token.ThrowIfCancellationRequested();
                return null;
            }

            return InvokeOnDispose.Create(() =>
            {
                m_Semaphore.Release();
            });
        }

        /// <summary>
        /// Dispose the synchronizer.
        /// </summary>
        public void Dispose() => m_Semaphore.Dispose();
    }
}
