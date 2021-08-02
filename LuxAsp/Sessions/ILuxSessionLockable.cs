using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Sessions
{
    public interface ILuxSessionLockable
    {
        /// <summary>
        /// Acquire the Session.
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        Task AcquireAsync(CancellationToken Token);

        /// <summary>
        /// Release the Session.
        /// </summary>
        /// <returns></returns>
        Task ReleaseAsync(CancellationToken Token);
    }
}