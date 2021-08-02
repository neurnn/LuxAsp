using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Sessions
{
    /// <summary>
    /// Session Store Worker interface.
    /// </summary>
    public interface ILuxSessionStoreWorker
    {
        /// <summary>
        /// Execute an work item in Background Worker.
        /// </summary>
        /// <param name="Workitem"></param>
        /// <returns></returns>
        ValueTask ExecuteAsync(Func<ValueTask> WorkItem);
    }
}
