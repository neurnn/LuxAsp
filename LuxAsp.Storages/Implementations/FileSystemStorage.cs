using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Implementations
{
    public class FileSystemStorage : IStorage
    {
        private static readonly Task<string> NULL_URL = Task.FromResult(null as string);
        private static readonly Task<bool> FALSE_TASK = Task.FromResult(false);
        private static readonly Task<bool> TRUE_TASK = Task.FromResult(true);
        private static readonly Task<string[]> EMPTY_LIST = Task.FromResult(new string[0]);
        private static readonly Task<Stream> NULL_STREAM = Task.FromResult(null as Stream);

        public FileSystemStorage(DirectoryInfo Directory, string BaseUrl = null)
        {
            this.BaseDirectory = Directory;
            this.BaseUrl = BaseUrl;
        }

        /// <summary>
        /// File System Storage will be always alive.
        /// </summary>
        public bool IsAlive => true;

        /// <summary>
        /// When Base URL set, this can generate url.
        /// </summary>
        public bool CanMakeUrl => !string.IsNullOrEmpty(BaseUrl);

        /// <summary>
        /// Directory to store blobs.
        /// </summary>
        public DirectoryInfo BaseDirectory { get; }

        /// <summary>
        /// Base URL.
        /// </summary>
        public string BaseUrl { get; }

        /// <summary>
        /// Make URL asynchronously.
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        public Task<string> MakeUrlAsync(string Path, CancellationToken Cancel = default)
        {
            if (BaseUrl is null) return NULL_URL;
            return Task.FromResult(string.Join('/', 
                BaseUrl.TrimEnd('/', '\\'), 
                Path.TrimStart('/', '\\')));
        }

        /// <summary>
        /// Test whether the Path string exists or not.
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        public Task<bool> ExistsAsync(string Path, CancellationToken Cancel = default)
        {
            if (!BaseDirectory.Exists)
                return FALSE_TASK;

            Path = System.IO.Path.Combine(BaseDirectory.FullName, Path.TrimStart('/', '\\'));
            return Task.FromResult(File.Exists(Path) || Directory.Exists(Path));
        }

        /// <summary>
        /// List the storage path's sub files.
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        public Task<string[]> ListAsync(string Path, CancellationToken Cancel = default)
        {
            if (!BaseDirectory.Exists)
                return EMPTY_LIST;

            Path = System.IO.Path.Combine(BaseDirectory.FullName, Path.TrimStart('/', '\\'));
            return Task.FromResult(Directory.GetFiles(Path)
                .Select(X => X.Substring(BaseDirectory.FullName.Length).TrimStart('/', '\\'))
                .ToArray());
        }

        /// <summary>
        /// Put the stream as the storage path asynchronously.
        /// When the target path indicates directory, this throws NotSupportedException.
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="DataStream"></param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        public async Task<bool> PutAsync(string Path, Stream DataStream, CancellationToken Cancel = default)
        {
            if (!BaseDirectory.Exists)
                return false;

            Path = System.IO.Path.Combine(BaseDirectory.FullName, Path.TrimStart('/', '\\'));
            var BasePath = System.IO.Path.GetDirectoryName(Path);

            try
            {
                if (!Directory.Exists(BasePath))
                     Directory.CreateDirectory(BasePath);

                var Attribute = File.GetAttributes(Path);
                if (Attribute.HasFlag(FileAttributes.Directory))
                    throw new NotSupportedException("the path indicates a directory.");

                if (File.Exists(Path))
                    File.Delete(Path);
            }
            catch(Exception e) when (!(e is NotSupportedException)) { return false; }

            try
            {
                using (var Stream = File.Open(Path, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                    await DataStream.CopyToAsync(Stream, Cancel);

                return true;
            }

            catch
            {
                try
                {
                    if (File.Exists(Path))
                        File.Delete(Path);
                }
                catch { }
            }

            return false;
        }

        /// <summary>
        /// Get the storage path as stream asynchronously.
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public Task<Stream> GetAsync(string Path, CancellationToken Cancel = default)
        {
            if (!BaseDirectory.Exists)
                return NULL_STREAM;

            Path = System.IO.Path.Combine(BaseDirectory.FullName, Path.TrimStart('/', '\\'));

            try
            {
                var Attribute = File.GetAttributes(Path);
                if (Attribute.HasFlag(FileAttributes.Directory))
                    return NULL_STREAM;

                return Task.FromResult<Stream>(File.Open(Path,
                    FileMode.Open, FileAccess.Read, FileShare.Read));
            }

            catch { }
            return NULL_STREAM;
        }

        /// <summary>
        /// Delete the storage path asynchronously.
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(string Path, CancellationToken Cancel = default)
        {
            if (!BaseDirectory.Exists)
                return FALSE_TASK;

            Path = System.IO.Path.Combine(BaseDirectory.FullName, Path.TrimStart('/', '\\'));

            try
            {
                var Attribute = File.GetAttributes(Path);
                if (Attribute.HasFlag(FileAttributes.Directory))
                {
                    Directory.Delete(Path, true);
                    return TRUE_TASK;
                }

                if (File.Exists(Path))
                    File.Delete(Path);

                return TRUE_TASK;
            }

            catch { }
            return FALSE_TASK;
        }
    }
}
