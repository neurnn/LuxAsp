using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp
{
    /// <summary>
    /// Database Migration interface.
    /// </summary>
    public interface IDatabaseMigrations
    {
        /// <summary>
        /// Wait Migration Completed asynchronously.
        /// </summary>
        /// <param name="Cancellation"></param>
        /// <returns></returns>
        Task WaitMigrationAsync(CancellationToken Cancellation = default);
    }
}
