using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LuxAsp.Sessions.Internals
{

    internal class LuxSessionStoreWorker : BackgroundService, ILuxSessionStoreWorker
    {
        private readonly Channel<Func<ValueTask>> m_Queue;

        public LuxSessionStoreWorker()
        {
            var Options = new BoundedChannelOptions(32)
            {
                FullMode = BoundedChannelFullMode.Wait
            };

            m_Queue = Channel.CreateBounded<Func<ValueTask>>(Options);
        }

        /// <summary>
        /// Execute an work item in Background Worker.
        /// </summary>
        /// <param name="Workitem"></param>
        /// <returns></returns>
        public async ValueTask ExecuteAsync(Func<ValueTask> WorkItem)
            => await m_Queue.Writer.WriteAsync(WorkItem ?? throw new ArgumentNullException(nameof(WorkItem)));

        /// <summary>
        /// Execute Background task.
        /// </summary>
        /// <param name="StoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken StoppingToken)
        {
            Func<ValueTask> WorkItem;
            Func<Func<ValueTask>, ValueTask> InvokeWorkItem
                = Debugger.IsAttached ? X => X() : Invoke;

            while (!StoppingToken.IsCancellationRequested)
            {
                try { WorkItem = await m_Queue.Reader.ReadAsync(StoppingToken); }
                catch { break; }

                try { await InvokeWorkItem(WorkItem); }
                catch(OperationCanceledException) { }
            }
        }

        private async ValueTask Invoke(Func<ValueTask> Item)
        {
            try { await Item(); }
            catch { }
        }
    }
}
