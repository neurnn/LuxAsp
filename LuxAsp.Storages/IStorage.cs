using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp
{
    /// <summary>
    /// Storage interface.
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Test whether the storage is still alive or not.
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// Supportness about making the url from the storage path.
        /// </summary>
        bool CanMakeUrl { get; }

        /// <summary>
        /// Make the external accessible URI if supported.
        /// When this operation not supported, this returns null instead of throwing exceptions.
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        Task<string> MakeUrlAsync(string Path, CancellationToken Cancel = default);

        /// <summary>
        /// Test whether the storage path exists or not.
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string Path, CancellationToken Cancel = default);

        /// <summary>
        /// List the storage path's sub files.
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        Task<string[]> ListAsync(string Path, CancellationToken Cancel = default);

        /// <summary>
        /// Put the stream as the storage path asynchronously.
        /// When the target path indicates directory, this throws NotSupportedException.
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="DataStream"></param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        Task<bool> PutAsync(string Path, Stream DataStream, CancellationToken Cancel = default);

        /// <summary>
        /// Get the storage path as stream asynchronously.
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        Task<Stream> GetAsync(string Path, CancellationToken Cancel = default);

        /// <summary>
        /// Delete the storage path asynchronously.
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(string Path, CancellationToken Cancel = default);
    }
}
