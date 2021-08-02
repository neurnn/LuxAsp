using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Sessions.Internals
{
    internal class LuxFileSessionStore : LuxMemorySessionStore
    {
        private string m_Path;

        /// <summary>
        /// Initialize a new File Session Store.
        /// </summary>
        /// <param name="Options"></param>
        public LuxFileSessionStore(LuxSessionOptions Options, ILuxSessionStoreWorker Worker)
            : base(Options, Worker)
        {
            if (!(Options.Properties["LUX_FSSESS_DIR"] is DirectoryInfo Dir))
            {
                var BinPath = Path.GetDirectoryName(typeof(LuxFileSessionStore).Assembly.Location);
                var FirstDir = Path.Combine(BinPath, "lux", "sessions");

                if (IsWritableDirectory(FirstDir))
                    Dir = new DirectoryInfo(FirstDir);

                else
                {
                    var SecondDir = Path.Combine(Path.GetTempPath(), "lux", "sessions");

                    if (!IsWritableDirectory(SecondDir))
                    {
                        throw new InvalidOperationException(
                            $"the candidate session directories are not writable.\n" +
                            $"1. {FirstDir}.\n2. {SecondDir}.\n" +
                            $"please check the permission or disk free-space.");
                    }

                    Dir = new DirectoryInfo(SecondDir);
                }
            }

            else if (!IsWritableDirectory(Dir.FullName))
            {
                throw new InvalidOperationException(
                    $"the specified session directories are not writable: {Dir.FullName}.\n" +
                    $"please check the permission or disk free-space.");
            }

            m_Path = Dir.FullName;
        }

        /// <summary>
        /// Determines whether the directory is writable or not.
        /// </summary>
        /// <param name="Dir"></param>
        /// <returns></returns>
        private static bool IsWritableDirectory(string Dir)
        {
            var TestFile = Path.Combine(Dir, ".test");

            try
            {
                if (!Directory.Exists(Dir))
                     Directory.CreateDirectory(Dir);

                if (File.Exists(TestFile))
                    File.Delete(TestFile);

                File.WriteAllText(TestFile, "available?");
                if (File.Exists(TestFile))
                {
                    File.Delete(TestFile);
                    return true;
                }
            }

            catch { }
            return false;
        }

        /// <summary>
        /// Try to restore the Session.
        /// </summary>
        /// <param name="Guid"></param>
        /// <param name="Session"></param>
        /// <returns></returns>
        protected override bool TryRestoreSession(Guid Guid, out MemorySession Session)
        {
            var File = new FileInfo(Path.Combine(m_Path, Guid.ToString() + ".bin"));

            if (File.Exists)
            {
                Session = new MemorySession()
                {
                    Guid = Guid, 
                    LastAccessTime = File.LastWriteTime
                };

                using (var Reader = new BinaryReader(File.OpenRead(), Encoding.UTF8, false))
                {
                    if (!Deserialize(Reader, Session))
                    {
                        try { File.Delete(); }
                        catch { }

                        Session = null;
                        return false;
                    }
                }

                try { File.Delete(); }
                catch { }

                return true;
            }

            Session = null;
            return false;
        }

        /// <summary>
        /// Deserialize the Session from Binary Reader.
        /// </summary>
        /// <param name="Reader"></param>
        /// <param name="Where"></param>
        /// <returns></returns>
        private bool Deserialize(BinaryReader Reader, MemorySession Where)
        {
            try
            {
                var Version = Reader.ReadInt16();
                if (Version != 1)
                    return false;

                var Count = Reader.ReadInt32();
                while (Count-- > 0)
                {
                    var Length = Reader.ReadInt32();
                    var Name = Reader.ReadString();

                    if (Length <= 0)
                        continue;

                    Where.Set(Name, Reader.ReadBytes(Length));
                }

                return true;
            }

            catch { }
            Where.Abandon();

            return false;
        }

        /// <summary>
        /// File Session supports swapping.
        /// </summary>
        protected override bool SupportsIdleSwapping => true;

        /// <summary>
        /// Try to Store the Idle Session to Disk.
        /// </summary>
        /// <param name="Guid"></param>
        /// <param name="Session"></param>
        /// <returns></returns>
        protected override bool TryStoreSession(Guid Guid, MemorySession Session)
        {
            var File = new FileInfo(Path.Combine(m_Path, Guid.ToString() + ".bin"));

            try
            {
                if (File.Exists)
                    File.Delete();
            }
            catch { return false; }

            using (var Writer = new BinaryWriter(File.OpenWrite(), Encoding.UTF8, false))
            {
                try
                {
                    Writer.Write((ushort)1);
                    Writer.Write(Session.Keys.Count());

                    foreach (var Each in Session.Keys)
                    {
                        var Bytes = Session.Get(Each);

                        Writer.Write(Bytes != null ? Bytes.Length : 0);
                        Writer.Write(Each);

                        if (Bytes.Length <= 0)
                            continue;

                        Writer.Write(Bytes);
                    }
                }

                catch { return false; }
                return true;
            }

        }
    }
}
